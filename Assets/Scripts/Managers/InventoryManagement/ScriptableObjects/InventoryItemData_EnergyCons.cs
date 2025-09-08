using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item - Energy Consumable")]
public class InventoryItemData_EnergyCons : InventoryItemData
{
    public int EnergyModifier = 0;

    public override bool UseItem()
    {
        base.UseItem();

        Debug.Log($"Replenish energy for: {EnergyModifier}");

        return true;
    }
}
