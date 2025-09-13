using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Frontend of the inventory
/// </summary>
public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem;

    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary;   // Pair up the UI slots with the system slots.

    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

    protected GraveDecorationManagement eventReceiver = null;

    /// <summary>
    /// Default values.
    /// </summary>
    protected virtual void Start()
    {

    }

    /// <summary>
    /// Implemented in child classes.
    /// </summary>
    /// <param name="invToDisplay"></param>
    public abstract void AssignSlot(InventorySystem invToDisplay, int offset);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="updatedSlot"></param>
    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if (slot.Value == updatedSlot)          // Slot value - the "under the hood" inventory slot.
            {
                slot.Key.UpdateUISlot(updatedSlot); // Slot key - the UI representation of the value.
            }
        }
    }

    /// <summary>
    /// Logic of the slot click
    /// </summary>
    /// <param name="clickedUISlot">Clicked slot</param>
    public void SlotClicked(InventorySlot_UI clickedUISlot, bool middleMouse)
    {
        mouseInventoryItem.takenFromSlot = clickedUISlot;

        // Pressed shift and clicked
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            /// TODO: Implement quick add to inventory
        }

        // Does the clicked slot have an item - Does the mouse have an item? 
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            // If player is holding shift key and is slot splittable? Split the stack.
            if (middleMouse && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
                eventReceiver?.InventorySlotChange(clickedUISlot.AssignedInventorySlot);
                return;
            }
            else // Pick up the item in the clicked slot
            {
                mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                clickedUISlot.ClearSlot();
                eventReceiver?.InventorySlotChange(clickedUISlot.AssignedInventorySlot);
                return;
            }
        }

        // clicked slot doesn't have an item - Mouse does have an item - place the mouse item into the empty slot.
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            if (!clickedUISlot.AssignedInventorySlot.CorrectType(mouseInventoryItem.AssignedInventorySlot.ItemData))
                return;

            // Enough room left in stack?
            if (clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, mouseInventoryItem.AssignedInventorySlot.ItemData))
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
            }
            else // Is the slot stack size + mouse stack size > the slot max stack size? If so, take from mouse. 
            {
                clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack, mouseInventoryItem.AssignedInventorySlot.ItemData);

                int remainingMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;

                clickedUISlot.AssignedInventorySlot.AssignItem(new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, leftInStack));
                clickedUISlot.UpdateUISlot();

                InventorySlot newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingMouse);
                mouseInventoryItem.ClearSlot();
                mouseInventoryItem.UpdateMouseSlot(newItem);

                eventReceiver?.InventorySlotChange(clickedUISlot.AssignedInventorySlot);
                return;
            }

            eventReceiver?.InventorySlotChange(clickedUISlot.AssignedInventorySlot);
            return;
        }

        // Both slots have an item - Decide what to do...
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;

            // Are both items the same? If so combine them.
            if (isSameItem && clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                eventReceiver?.InventorySlotChange(clickedUISlot.AssignedInventorySlot);
                return;
            }
            else if (isSameItem && !clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack)) // Is the slot stack size + mouse stack size > the slot max stack size? If so, take from mouse. 
            {
                if (leftInStack < 1)            // Stack is full so swap the items.
                {
                    if (!clickedUISlot.AssignedInventorySlot.CorrectType(mouseInventoryItem.AssignedInventorySlot.ItemData))
                        return;
                    else if (clickedUISlot.AssignedInventorySlot.inventorySlotType == InventorySlotType.Decoration)
                        return;

                    eventReceiver?.InventorySlotChange(clickedUISlot.AssignedInventorySlot);
                    SwapSlots(clickedUISlot);  
                }
                else    // Slot is not at max, so take what's need from the mouse inventory
                {
                    int remainingMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;

                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    InventorySlot newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingMouse, eventReceiver);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(newItem);
                    eventReceiver?.InventorySlotChange(clickedUISlot.AssignedInventorySlot);
                    return;
                }
            }
            else  // If different items, then swap the items.
            {
                if (!clickedUISlot.AssignedInventorySlot.CorrectType(mouseInventoryItem.AssignedInventorySlot.ItemData))
                    return;
                else if (clickedUISlot.AssignedInventorySlot.inventorySlotType == InventorySlotType.Decoration)
                    return;

                eventReceiver?.InventorySlotChange(clickedUISlot.AssignedInventorySlot);
                SwapSlots(clickedUISlot);
                return;
            }
        }
    }

    /// <summary>
    /// Swap the mouse item with the slot item
    /// </summary>
    /// <param name="clickedSlot">Clicked slot</param>
    private void SwapSlots(InventorySlot_UI clickedSlot)
    {
        InventorySlot clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();

        mouseInventoryItem.UpdateMouseSlot(clickedSlot.AssignedInventorySlot);

        clickedSlot.ClearSlot();
        clickedSlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedSlot.UpdateUISlot();
    }
}
