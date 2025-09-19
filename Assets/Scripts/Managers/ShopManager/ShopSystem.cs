using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShopSystem
{
    [SerializeField] public List<ShopSlot> _shopInventory;
    [SerializeField] public MoneyAmount _availableGold;
    [SerializeField] private float _buyMarkUp;
    [SerializeField] private float _sellMarkUp;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    /// <param name="gold"></param>
    /// <param name="buyMarkUp"></param>
    /// <param name="sellMarkUp"></param>
    public ShopSystem(int size, MoneyAmount gold, float buyMarkUp, float sellMarkUp)
    {
        this._availableGold = gold;
        this._buyMarkUp = buyMarkUp;
        this._sellMarkUp = sellMarkUp;

        SetShopSize(size);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    private void SetShopSize(int size)
    {
        _shopInventory = new List<ShopSlot>(size);

        for (int i = 0; i < size; i++)
        {
            _shopInventory.Add(new ShopSlot());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="amount"></param>
    public void AddToShop(InventoryItemData data, int amount)
    {
        if (ContainsItem(data, out ShopSlot shopSlot))
        {
            shopSlot.AddToStack(amount);
        }

        var freeSlot = GetFreeSlot();
        freeSlot.AssignItem(data, amount);
    }

    private ShopSlot GetFreeSlot()
    {
        var freeSlot = _shopInventory.FirstOrDefault(i => i.ItemData == null);

        if (freeSlot == null)
        {
            freeSlot = new ShopSlot();
            _shopInventory.Add(freeSlot);
        }

        return freeSlot;
    }

    /// <summary>
    /// If the inventory already contains the data.
    /// Returns the slots holding the data.
    /// </summary>
    /// <param name="itemToAdd">Data</param>
    /// <param name="invSlots">Slots holding the data</param>
    /// <returns></returns>
    public bool ContainsItem(InventoryItemData itemToAdd, out ShopSlot shopSlot)
    {
        shopSlot = _shopInventory.Find(i => i.ItemData == itemToAdd);
        return shopSlot != null;
    }
}
