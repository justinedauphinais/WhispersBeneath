using UnityEngine;

public class FullBackpackInventory : MonoBehaviour
{
    [SerializeField] public InventorySlot_UI[] hotbarSlots;
    [SerializeField] public DynamicInventoryDisplay backpack;
    [SerializeField] public InventoryUIController inventoryController;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invToDisplay"></param>
    /// <param name="offset"></param>
    public void RefreshDynamicInventory(InventorySystem invToDisplay, int offset)
    {
        //inventorySystem = invToDisplay;

        int index = 0;
        foreach (InventorySlot_UI slot in hotbarSlots)
        {
            slot.Init(invToDisplay.InventorySlots[index]);
            index++;
        }

        backpack.RefreshDynamicInventory(invToDisplay, offset);
    }

    public void OnDisable()
    {
        inventoryController.UpdateFromBackpack();
    }
}
