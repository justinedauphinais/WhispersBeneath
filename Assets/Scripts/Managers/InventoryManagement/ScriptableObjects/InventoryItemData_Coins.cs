using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item - Coin")]
public class InventoryItemData_Coins : InventoryItemData
{
    public CoinType coinType;

    public override bool BoughtItem()
    {
        // Delete + Add Money to Character

        return base.BoughtItem();
    }
}

public enum CoinType
{
    Bronze,
    Silver,
    Gold
}