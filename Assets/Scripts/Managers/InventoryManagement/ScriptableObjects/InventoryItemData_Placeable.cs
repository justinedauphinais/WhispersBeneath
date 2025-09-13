using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item - Placeable")]
public class InventoryItemData_Placeable : InventoryItemData
{
    [SerializeField]
    public Vector2Int Size = Vector2Int.zero;       /// Size of the object on the grid

    [SerializeField]
    public PlaceableType placeableType;

    /// <summary>
    /// If the object is placeable - Calculated by if it has a size.
    /// If it is, when used, we place it down.
    /// If it isn't, when used, we consume.
    /// </summary>
    /// <returns></returns>
    protected bool IsPlaceable()
    {
        return Size != Vector2Int.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool UseItem()
    {
        base.UseItem();

        StateManager stateManager = GameObject.FindGameObjectWithTag("StateManager").GetComponent<StateManager>();
        stateManager.ActivateBuildingMode(this);

        return false;
    }
}

public enum PlaceableType
{
    Normal,
    ConnectedTile,
    Building
}
