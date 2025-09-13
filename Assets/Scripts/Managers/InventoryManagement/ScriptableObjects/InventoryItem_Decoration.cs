using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item - Decoration")]
public class InventoryItem_Decoration : InventoryItemData
{
    public int MaxStackSizeOnDecorationSlot;
    public int DecorationLevel;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool UseItem()
    {
        base.UseItem();
        return false;
    }
}
