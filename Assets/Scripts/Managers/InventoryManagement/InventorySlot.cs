using System;
using UnityEngine;
using UnityEngine.Events;

public enum InventorySlotType
{
    Default,
    Decoration
}

/// <summary>
/// Backend inventory slot that holds the amount of the data (stack size)
/// </summary>
[System.Serializable]
public class InventorySlot : ISerializationCallbackReceiver
{
    [Header("Inventory Item Information")]
    [SerializeField] protected int itemID = -1;
    [SerializeField] protected int stackSize;                 // Current stack size - how many of the data do we have?

    [NonSerialized] protected InventoryItemData itemData;     // Reference to the data

    public InventoryItemData ItemData => this.itemData;

    public int StackSize => this.stackSize;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public bool InInventory = true;
    public InventorySlotType inventorySlotType = InventorySlotType.Default;

    /// <summary>
    /// Default constructor to make an empty inventory slot.
    /// </summary>
    public InventorySlot(InventorySlotType type = InventorySlotType.Default)
    {
        ClearSlot();
        inventorySlotType = type;
    }

    /// <summary>
    /// Constructor to make an occupied inventory slot.
    /// </summary>
    /// <param name="source">Data to copy</param>
    /// <param name="amount">How many of the data do we have?</param>
    public InventorySlot(InventoryItemData source, int amount, InventorySlotType type = InventorySlotType.Default)
    {
        itemData = source;
        itemID = itemData.ID;
        stackSize = amount;
        inventorySlotType = type;
    }

    /// <summary>
    /// Constructor to make an occupied inventory slot.
    /// </summary>
    /// <param name="source">Data to copy</param>
    /// <param name="amount">How many of the data do we have?</param>
    public InventorySlot(InventoryItemData source, int amount, GraveDecorationManagement eventReceiver, InventorySlotType type = InventorySlotType.Default)
    {
        itemData = source;
        itemID = itemData.ID;
        stackSize = amount;
        inventorySlotType = type;
        
        if (eventReceiver) OnInventorySlotChanged += eventReceiver.InventorySlotChange;
    }

    /// <summary>
    /// Clears the slot 
    /// </summary>
    public void ClearSlot()
    {
        itemData = null;
        itemID = -1;
        stackSize = -1;
        OnInventorySlotChanged?.Invoke(this);
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
    /// Updates slot directly.
    /// </summary>
    /// <param name="data">Item</param>
    /// <param name="amount">Amount of the item</param>
    public void UpdateInventorySlot(InventoryItemData data, int amount)
    {
        itemData = data;
        itemID = itemData.ID;
        stackSize = amount;
        OnInventorySlotChanged?.Invoke(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool CorrectType(InventoryItemData data)
    {
        if (inventorySlotType == InventorySlotType.Default)
            return true;
        else if (inventorySlotType == InventorySlotType.Decoration)
        {
            if (data.ItemDataType == InventoryItemDataType.Decoration)
                return true;
            else return false;
        }

        return true;
    }

    /// <summary>
    /// Would there be enough room in the stack for the amount we're trying to add.
    /// Returns the amount remaining from the amount to add
    /// </summary>
    /// <param name="amountToAdd">Amount to add</param>
    /// <param name="amountRemaining">Amount remaining from the amount to add</param>
    /// <returns></returns>
    public virtual bool RoomLeftInStack(int amountToAdd, out int amountRemaining, InventoryItemData item = null)
    {
        amountRemaining = 0;

        if (inventorySlotType == InventorySlotType.Default)
            amountRemaining = ItemData.MaxStackSize - stackSize;
        else if (inventorySlotType == InventorySlotType.Decoration)
        {
            if (itemData == null)
            {
                amountRemaining = ((InventoryItem_Decoration)item).MaxStackSizeOnDecorationSlot;
            }
            else
            {
                amountRemaining = ((InventoryItem_Decoration)ItemData).MaxStackSizeOnDecorationSlot - stackSize;
            }
        }
        
        return EnoughRoomLeftInStack(amountToAdd, (InventoryItem_Decoration)ItemData);
    }

    /// <summary>
    /// Would there be enough room in the stack for the amount we're trying to add.
    /// </summary>
    /// <param name="amountToAdd">Amount to add</param>
    /// <returns></returns>
    public virtual bool EnoughRoomLeftInStack(int amountToAdd, InventoryItemData item = null)
    {
        if (inventorySlotType == InventorySlotType.Default)
            if (itemData == null || itemData != null && stackSize + amountToAdd <= itemData.MaxStackSize) return true;
            else return false;
        else if (inventorySlotType == InventorySlotType.Decoration)
            if (itemData == null)
            {
                if (item == null)
                    return true;
                else
                {
                    if (stackSize + amountToAdd <= ((InventoryItem_Decoration)item).MaxStackSizeOnDecorationSlot)
                        return true;
                    else 
                        return false;
                }
            }
            else if (stackSize + amountToAdd <= ((InventoryItem_Decoration)ItemData).MaxStackSizeOnDecorationSlot) 
                return true;
            else return false;
        else return false;
    }

    /// <summary>
    /// Add to stack.
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void AddToStack(int amount)
    {
        stackSize += amount;
        OnInventorySlotChanged?.Invoke(this);
    }

    /// <summary>
    /// Remove from stack.
    /// </summary>
    /// <param name="amount">Amount to remove</param>
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
        OnInventorySlotChanged?.Invoke(this);
    }

    /// <summary>
    /// Split the stack in half if enough to split. 
    /// Returns if split or not.
    /// Returns second half of the split stack.
    /// </summary>
    /// <param name="splitStack">Second half of the split stack</param>
    /// <returns></returns>
    public bool SplitStack(out InventorySlot splitStack)
    {
        // Is there enough to actually split? If not, return false. 
        if (stackSize <= 1)
        {
            splitStack = null;
            return false;
        }

        // Get half the stack.
        int halfStack = Mathf.RoundToInt(stackSize / 2);
        RemoveFromStack(halfStack);

        // Creates a new copy of this slot with half the stack size. 
        splitStack = new InventorySlot(itemData, halfStack);
        OnInventorySlotChanged?.Invoke(this);
        return true;
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
        if (itemID == -1) return;

        var db = Resources.Load<ItemDatabase>("Database");
        itemData = db.GetItem(itemID);
    }
}
