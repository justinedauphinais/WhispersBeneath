using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class StaticInventoryDisplay : InventoryDisplay
{
    [Header("Inventory")]
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] public InventorySlot_UI[] slots;

    [Header("Selected / Unselected slots sprites")]
    [SerializeField] private Sprite normalSlot;
    [SerializeField] private Sprite selectedSlot;

    private int selectedIndex = 0;

    protected override void Start()
    {
        base.Start();

        RefreshStaticDisplay();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChanged += RefreshStaticDisplay;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChanged -= RefreshStaticDisplay;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RefreshStaticDisplay()
    {
        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else Debug.LogWarning($"No inventory assigned to {this.gameObject.name}");

        AssignSlot(inventorySystem, 0);
    }

    /// <summary>
    /// Assign slot
    /// </summary>
    /// <param name="invToDisplay"></param>
    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        for (int i = 0; i < inventoryHolder.Offset; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public void SetIndex(int index)
    {
        slots[selectedIndex].SetSprite(normalSlot);
        selectedIndex = index;
        slots[selectedIndex].SetSprite(selectedSlot);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ScrollUp()
    {
        int index = selectedIndex + 1;

        if (index == 10) index = 0;

        SetIndex(index);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ScrollDown()
    {
        int index = selectedIndex - 1;

        if (index == -1) index = 9;

        SetIndex(index);
    }

    /// <summary>
    /// 
    /// </summary>
    public void UseItem()
    {
        if (slots[selectedIndex].AssignedInventorySlot.ItemData != null)
        {
            if (slots[selectedIndex].AssignedInventorySlot.ItemData.UseItem())
            {
                slots[selectedIndex].AssignedInventorySlot.RemoveFromStack(1);
                
                if (slots[selectedIndex].AssignedInventorySlot.StackSize == 0)
                {
                    slots[selectedIndex].AssignedInventorySlot.ClearSlot();
                }

                slots[selectedIndex].UpdateUISlot();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateAll()
    {
        foreach (InventorySlot_UI slot in slots)
        {
            slot.UpdateUISlot();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public InventorySlot GetSelectedItem()
    {
        return slots[selectedIndex].AssignedInventorySlot;
    }
}
