using System;
using UnityEngine;

public class ItemSlot : ISerializationCallbackReceiver
{
    [Header("Inventory Item Information")]
    [SerializeField] protected int itemID = -1;
    [SerializeField] protected int stackSize;                 // Current stack size - how many of the data do we have?

    [NonSerialized] protected InventoryItemData itemData;     // Reference to the data

    public InventoryItemData ItemData => this.itemData;

    public int StackSize => this.stackSize;

    private void Start()
    {
        if (itemID == -1) return;

        var db = Resources.Load<ItemDatabase>("Database");
        itemData = db.GetItem(itemID);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnBeforeSerialize()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public void OnAfterDeserialize()
    {
        
    }

    /// <summary>
    /// Clears the slot 
    /// </summary>
    public virtual void ClearSlot()
    {
        itemData = null;
        itemID = -1;
        stackSize = -1;
    }

    /// <summary>
    /// Assigns an item to the slot.
    /// </summary>
    /// <param name="invSlot">Item</param>
    public void AssignItem(InventorySlot invSlot)
    {
        if (itemData == invSlot.ItemData)   // Does the slot contain the same item? Add 
        {
            AddToStack(invSlot.StackSize);
        }
        else                                // Overwrite slot with the inventory slot that we're passing in.
        {
            itemData = invSlot.itemData;
            itemID = itemData.ID;
            stackSize = 0;
            AddToStack(invSlot.stackSize);
        }
    }

    /// <summary>
    /// Assigns an item to the slot.
    /// </summary>
    /// <param name="invSlot">Item</param>
    public void AssignItem(InventoryItemData data, int amount)
    {
        if (itemData == data)   // Does the slot contain the same item? Add 
        {
            AddToStack(amount);
        }
        else                                // Overwrite slot with the inventory slot that we're passing in.
        {
            itemData = data;
            itemID = data.ID;
            stackSize = 0;
            AddToStack(amount);
        }
    }

    /// <summary>
    /// Add to stack.
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public virtual void AddToStack(int amount)
    {
        stackSize += amount;
    }
}
