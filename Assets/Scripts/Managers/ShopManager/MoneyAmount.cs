using System;
using UnityEngine;

[Serializable]
public class MoneyAmount
{
    public int bronzeAmount;
    public int silverAmount;
    public int goldAmount;

    public MoneyAmount()
    {
        bronzeAmount = 0;
        silverAmount = 0;
        goldAmount = 0;
    }

    public MoneyAmount(int bronzeAmount, int silverAmount, int goldAmount)
    {
        this.bronzeAmount = bronzeAmount;
        this.silverAmount = silverAmount;
        this.goldAmount = goldAmount;
        Normalize();
    }

    public static MoneyAmount operator +(MoneyAmount a, MoneyAmount b)
    {
        a.bronzeAmount += b.bronzeAmount;
        a.silverAmount += b.silverAmount;
        a.goldAmount += b.goldAmount;
        return a;
    }

    // ---------- Helpers ----------
    public int ToTotalBronze() => bronzeAmount + 10 * silverAmount + 100 * goldAmount;

    public static MoneyAmount FromTotalBronze(int total)
    {
        if (total < 0) total = 0;
        int gold = total / 100;
        total %= 100;
        int silver = total / 10;
        total %= 10;
        int bronze = total;
        return new MoneyAmount(bronze, silver, gold);
    }

    public MoneyAmount Clone() => new MoneyAmount(bronzeAmount, silverAmount, goldAmount);

    public void Normalize()
    {
        // carry up
        if (bronzeAmount >= 10)
        {
            silverAmount += bronzeAmount / 10;
            bronzeAmount %= 10;
        }
        if (silverAmount >= 10)
        {
            goldAmount += silverAmount / 10;
            silverAmount %= 10;
        }
        // prevent negatives trickling the wrong way
        if (bronzeAmount < 0 || silverAmount < 0 || goldAmount < 0)
        {
            // fallback clamp; callers should not pass negatives
            int total = Math.Max(0, ToTotalBronze());
            var norm = FromTotalBronze(total);
            bronzeAmount = norm.bronzeAmount;
            silverAmount = norm.silverAmount;
            goldAmount = norm.goldAmount;
        }
    }

    static void AddInPlace(MoneyAmount a, MoneyAmount b)
    {
        a.bronzeAmount += b.bronzeAmount;
        a.silverAmount += b.silverAmount;
        a.goldAmount += b.goldAmount;
        a.Normalize();
    }

    static bool TrySubtractInPlace(MoneyAmount a, MoneyAmount b)
    {
        // Subtract with borrow using totals (simple and safe).
        int res = a.ToTotalBronze() - b.ToTotalBronze();
        if (res < 0) return false;
        var norm = FromTotalBronze(res);
        a.bronzeAmount = norm.bronzeAmount;
        a.silverAmount = norm.silverAmount;
        a.goldAmount   = norm.goldAmount;
        return true;
    }

    // Greedy: take highest coins first until contributed value >= target.
    static MoneyAmount TakeForPaymentGreedy(MoneyAmount wallet, int targetBronze, out int contributed)
    {
        var pay = new MoneyAmount();
        contributed = 0;

        // Take gold
        int need = Math.Max(0, targetBronze - contributed);
        int goldToUse = Math.Min(wallet.goldAmount, (int)Math.Ceiling(need / 100f));
        // We might not need that many if smaller coins cover the rest; we’ll refine below.

        // More robust: take as many as needed step by step:
        for (int i = 0; i < wallet.goldAmount && contributed < targetBronze; i++)
        {
            pay.goldAmount++; contributed += 100;
        }
        // Silver
        for (int i = 0; i < wallet.silverAmount && contributed < targetBronze; i++)
        {
            pay.silverAmount++; contributed += 10;
        }
        // Bronze
        int bronzeNeeded = Math.Max(0, targetBronze - contributed);
        int bronzeToUse = Math.Min(wallet.bronzeAmount, bronzeNeeded);
        pay.bronzeAmount += bronzeToUse;
        contributed += bronzeToUse;

        // If still not enough (because we ran out of bronze), we may have to add an extra silver/gold
        // (this overpays and expects change).
        while (contributed < targetBronze)
        {
            if (wallet.silverAmount - pay.silverAmount > 0)
            {
                pay.silverAmount++; contributed += 10;
            }
            else if (wallet.goldAmount - pay.goldAmount > 0)
            {
                pay.goldAmount++; contributed += 100;
            }
            else
            {
                break; // cannot cover
            }
        }

        pay.Normalize();
        return pay;
    }

    // Try to build exact change <= changeDue using ONLY coins whose value <= remaining.
    // Uses greedy largest->smallest (optimal here due to 1,10,100 base-10).
    static MoneyAmount MakeExactChangeGreedy(MoneyAmount till, int changeDue, out int made)
    {
        var change = new MoneyAmount();
        made = 0;
        if (changeDue <= 0) return change;

        // gold usable only if <= remaining (i.e., remaining >= 100)
        if (changeDue >= 100 && till.goldAmount > 0)
        {
            int use = Math.Min(till.goldAmount, changeDue / 100);
            change.goldAmount = use;
            made += use * 100;
            changeDue -= use * 100;
        }
        if (changeDue >= 10 && till.silverAmount > 0)
        {
            int use = Math.Min(till.silverAmount, changeDue / 10);
            change.silverAmount = use;
            made += use * 10;
            changeDue -= use * 10;
        }
        if (changeDue >= 1 && till.bronzeAmount > 0)
        {
            int use = Math.Min(till.bronzeAmount, changeDue);
            change.bronzeAmount = use;
            made += use;
            changeDue -= use;
        }

        change.Normalize();
        return change;
    }

    // ---------- Your existing method ----------
    public static MoneyAmount CalculateCost(int price, int amount)
    {
        int totalBronze = price * amount;
        return FromTotalBronze(totalBronze);
    }

    // ---------- Transaction API ----------

    /// <summary>
    /// Preview what would happen if the player buys an item for 'price' (in bronze).
    /// Does not mutate inputs. Computes the proposed new balances, the change given,
    /// and how much the player would lose if exact change can't be made.
    /// Returns true if the player has enough total value to cover 'price' (regardless of change).
    /// </summary>
    public static bool PreviewTransaction(
        MoneyAmount currentShopkeeper,
        MoneyAmount currentPlayer,
        int price,
        out MoneyAmount newShopkeeper,
        out MoneyAmount newPlayer,
        out MoneyAmount amountLost,
        out MoneyAmount changeGiven)
    {
        newShopkeeper = currentShopkeeper.Clone();
        newPlayer = currentPlayer.Clone();
        amountLost = new MoneyAmount();
        changeGiven = new MoneyAmount();

        if (price <= 0) return true; // free or invalid price → nothing happens

        int playerTotal = currentPlayer.ToTotalBronze();
        if (playerTotal < price)
        {
            // Cannot afford
            return false;
        }

        // 1) Decide what the player hands over (allow overpay, expecting change).
        int contributed;
        MoneyAmount payment = TakeForPaymentGreedy(currentPlayer, price, out contributed);

        // 2) Add payment to till (shopkeeper after receiving money).
        MoneyAmount tillAfterPayment = newShopkeeper.Clone();
        AddInPlace(tillAfterPayment, payment);

        // 3) Compute exact change if possible.
        int changeDue = contributed - price;
        int changeMade = 0;
        if (changeDue > 0)
        {
            changeGiven = MakeExactChangeGreedy(tillAfterPayment, changeDue, out changeMade);
        }

        int loss = Math.Max(0, changeDue - changeMade);
        amountLost = FromTotalBronze(loss);

        // 4) Build tentative new balances (do not mutate originals)
        // Player = player - payment + changeGiven (but if loss>0, they just don't get that part)
        newPlayer = currentPlayer.Clone();
        TrySubtractInPlace(newPlayer, payment);
        AddInPlace(newPlayer, changeGiven);

        // Shopkeeper = shop + payment - changeGiven
        newShopkeeper = currentShopkeeper.Clone();
        AddInPlace(newShopkeeper, payment);
        TrySubtractInPlace(newShopkeeper, changeGiven);

        // Return true (can afford). Caller decides whether to accept loss > 0.
        return true;
    }

    /// <summary>
    /// Applies the transaction by mutating the provided shopkeeper & player values
    /// if either exact change is possible OR the caller sets acceptLoss = true.
    /// Returns true on success, false if player can't afford or loss not accepted.
    /// </summary>
    public static bool ApplyTransaction(
        ref MoneyAmount shopkeeper,
        ref MoneyAmount player,
        int price,
        bool acceptLoss,
        out MoneyAmount amountLost)
    {
        amountLost = new MoneyAmount();
        MoneyAmount newShop, newPlayer, loss, _change;
        bool canAfford = PreviewTransaction(shopkeeper, player, price, out newShop, out newPlayer, out loss, out _change);
        if (!canAfford) return false;

        int lossTotal = loss.ToTotalBronze();
        if (lossTotal > 0 && !acceptLoss)
        {
            // Caller should show popup with 'loss' and ask.
            amountLost = loss;
            return false;
        }

        // Commit
        shopkeeper = newShop;
        player = newPlayer;
        amountLost = loss;
        return true;
    }

    // ---------- Optional: keep your original signature by adding an overload ----------
    // NOTE: Because your original signature has only 'out' params, we can't read the starting
    // balances from those. So we add parameters for the CURRENT balances and an acceptLoss flag.
    public static bool Transaction(
        MoneyAmount currentShopkeeper,
        MoneyAmount currentPlayer,
        int price,
        bool acceptLoss,
        out MoneyAmount shopkeeper,
        out MoneyAmount player,
        out MoneyAmount amountLost)
    {
        shopkeeper = currentShopkeeper.Clone();
        player = currentPlayer.Clone();

        // Use ApplyTransaction to handle logic.
        bool result = ApplyTransaction(ref shopkeeper, ref player, price, acceptLoss, out amountLost);
        return result;
    }
}
