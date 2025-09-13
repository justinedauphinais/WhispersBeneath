using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

/// <summary>
/// Holds the list of the inventory slots and the manager of placing the items 
/// </summary>
[System.Serializable]
public class InventorySystem
{
    [Header("Inventory Slots List")]
    [SerializeField] private List<InventorySlot> inventorySlots;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public List<InventorySlot> InventorySlots => this.inventorySlots;
    public int InventorySize => inventorySlots.Count;

    public InventorySlotType type = InventorySlotType.Default;

    /// <summary>
    /// Constructor that sets the amount of slots.
    /// </summary>
    /// <param name="size">Amount of slots</param>
    public InventorySystem(int size, bool addEvents = false, InventorySlotType type = InventorySlotType.Default)
    {
        this.inventorySlots = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            this.inventorySlots.Add(new InventorySlot(type));
            
            if (addEvents) this.inventorySlots[i].OnInventorySlotChanged += OnInventorySlotChanged;
        }
    }

    /// <summary>
    /// Add data to inventory
    /// </summary>
    /// <param name="itemToAdd">Data to add</param>
    /// <param name="amount">Amount of data to add</param>
    /// <returns></returns>
    public bool AddToInventory(InventoryItemData itemToAdd, int amount)
    {
        if (!CorrectType(itemToAdd)) return false;

        if (ContainsItem(itemToAdd, out List<InventorySlot> invSlots)) // Check whether item exists in inventory
        {
            foreach (InventorySlot slot in invSlots)
            {
                if (slot.EnoughRoomLeftInStack(amount))
                {
                    slot.AddToStack(amount);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }

        }

        if (HasFreeSlot(out InventorySlot freeSlot)) // Gets the first available slot
        {
            if (freeSlot.EnoughRoomLeftInStack(amount))
            {
                freeSlot.UpdateInventorySlot(itemToAdd, amount);
                OnInventorySlotChanged?.Invoke(freeSlot);
                return true;
            }
            // Add implementation to only take what can fill the stack, and check for another free slot to put the remainder in.
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemToAdd"></param>
    /// <returns></returns>
    public bool CorrectType(InventoryItemData itemToAdd)
    {
        if (type == InventorySlotType.Default)
            return true;
        else if (type == InventorySlotType.Decoration)
        {
            if (itemToAdd.ItemDataType == InventoryItemDataType.Decoration)
                return true;
            else
                return false;
        }

        return true;
    }

    /// <summary>
    /// If the inventory already contains the data.
    /// Returns the slots holding the data.
    /// </summary>
    /// <param name="itemToAdd">Data</param>
    /// <param name="invSlots">Slots holding the data</param>
    /// <returns></returns>
    public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlots)
    {
        // If they do, then get a list of all of them.
        invSlots = this.inventorySlots.Where(i => i.ItemData == itemToAdd).ToList();

        // If they do, return true, if not return false.
        return invSlots == null ? false : true;
    }

    /// <summary>
    /// If the inventory has free (empty) slots.
    /// Returns the next free (empty) slot.
    /// </summary>
    /// <param name="freeSlot">Next free (empty) slot</param>
    /// <returns></returns>
    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        // Get the first free slot.
        freeSlot = this.inventorySlots.FirstOrDefault(i => i.ItemData == null);

        return freeSlot == null ? false : true;
    }
}
