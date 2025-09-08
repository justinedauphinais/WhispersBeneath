using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected InventorySlot_UI slotPrefab;

    /// <summary>
    /// 
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    private void ClearSlots()
    {
        foreach (Transform item in transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        if (slotDictionary != null) slotDictionary.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invToDisplay"></param>
    public void RefreshDynamicInventory(InventorySystem invToDisplay, int offset)
    {
        ClearSlots();
        inventorySystem = invToDisplay;
        if (inventorySystem != null) inventorySystem.OnInventorySlotChanged += UpdateSlot;
        AssignSlot(invToDisplay, offset);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invToDisplay"></param>
    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        if (invToDisplay == null) return;

        for (int i = offset; i < invToDisplay.InventorySize; i++)
        {
            InventorySlot_UI uiSlot = Instantiate(slotPrefab, transform);
            slotDictionary.Add(uiSlot, invToDisplay.InventorySlots[i]);
            uiSlot.Init(invToDisplay.InventorySlots[i]);
            uiSlot.UpdateUISlot();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        if (inventorySystem != null) inventorySystem.OnInventorySlotChanged -= UpdateSlot;
    }
}
