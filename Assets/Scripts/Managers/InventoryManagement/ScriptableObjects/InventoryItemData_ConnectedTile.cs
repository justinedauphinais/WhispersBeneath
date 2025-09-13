using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item - Connected Tile")]
public class InventoryItemData_ConnectedTile : InventoryItemData_Placeable
{
    [SerializeField] private GameObject postMesh, endMesh, cornerMesh, straightMesh, tJunctionMesh, crossMesh;

    private ConnectableTile connectedTile;

    /// <summary>
    /// 
    /// </summary>
    public void Initialize()
    {
        connectedTile = new ConnectableTile(postMesh, endMesh, cornerMesh, straightMesh, tJunctionMesh, crossMesh);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public GameObject GetGameObjectAt(ConnectableTile.TileType tileType)
    {
        return connectedTile.GetMeshAt(tileType);
    }
}
