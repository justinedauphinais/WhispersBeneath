using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ItemDatabase database;

    [SerializeField] private InventoryItemData_Placeable selectedObject = null;

    [SerializeField] private GridData floorData, furnitureData;

    [SerializeField] private GameObject previewRenderer;

    [SerializeField] private Material[] materials;

    [SerializeField] private GameObject objectsParent;

    private InventoryItemData_ConnectedTile connectedTile;

    private List<GameObject> placedGameObjects = new List<GameObject>();

    private MeshRenderer gridVisualization;

    private Vector3Int lastGridPosition = Vector3Int.zero;

    private bool IsActive = false;

    public event Action<Vector3Int> PositionChanged;

    // Connectable tiles
    private bool NeedToReset = false;
    public List<int> ObjectIndexToReset = new List<int>();
    public ConnectableTile.TileType tileType = ConnectableTile.TileType.Post;
    public Quaternion rotation = Quaternion.Euler(0, 0, 0);

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        floorData = new GridData();
        furnitureData = new GridData();
        gridVisualization = grid.GetComponent<MeshRenderer>();

        StopPlacement();

        PickableObject[] children = objectsParent.GetComponentsInChildren<PickableObject>();

        foreach (PickableObject child in children)
        {
            placedGameObjects.Add(child.gameObject);
            Vector3 pos = child.transform.position;
            pos.y = 0;
            Vector3Int posOnGrid = grid.WorldToCell(pos);
            child.positionOnGrid = grid.WorldToCell(pos);
            furnitureData.AddObjectAt(posOnGrid, child.inventoryItemData.Size, child.inventoryItemData.ID, placedGameObjects.Count - 1);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObject = (InventoryItemData_Placeable)database.GetItem(ID);

        if (selectedObject == null)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }

        // If connectable tile - Else normal
        switch (selectedObject.placeableType)
        {
            case PlaceableType.ConnectedTile: 
                connectedTile = (InventoryItemData_ConnectedTile)database.GetItem(ID);
                connectedTile.Initialize();

                previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.Cross).GetComponent<MeshFilter>().sharedMesh ;
                previewRenderer.transform.position = connectedTile.GetGameObjectAt(ConnectableTile.TileType.Cross).transform.position;

                gridVisualization.enabled = true;
                cellIndicator.SetActive(true);

                inputManager.OnClicked += PlaceStructure;
                inputManager.OnExit += StopPlacement;
                inputManager.OnUpdate += OnUpdateConnectedType;

                return;

            case PlaceableType.Normal:
                previewRenderer.GetComponent<MeshFilter>().mesh = selectedObject.ItemPrefab.GetComponent<MeshFilter>().sharedMesh;
                previewRenderer.transform.position = selectedObject.ItemPrefab.transform.position;

                gridVisualization.enabled = true;
                cellIndicator.SetActive(true);

                inputManager.OnClicked += PlaceStructure;
                inputManager.OnExit += StopPlacement;
                inputManager.OnUpdate += OnUpdateNormalMesh;
                inputManager.OnRotate += Rotate;

                return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    public void StartPlacement(InventoryItemData_Placeable item)
    {
        StopPlacement();
        selectedObject = item;

        gridVisualization.enabled = true;
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        inputManager.OnUpdate += OnUpdateNormalMesh;
        inputManager.OnRotate += Rotate;
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlaceStructure()
    {
        NeedToReset = false;

        foreach (int i in ObjectIndexToReset)
        {
            placedGameObjects[i].GetComponent<ConnectableTile>().FinalizeMesh();
        }

        ObjectIndexToReset.Clear();

        if (inputManager.IsPointedOverUI()) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (!CheckPlacementValidity(gridPosition, selectedObject))
            return;

        GameObject gameObject = Instantiate(selectedObject.ItemPrefab, 
                                            previewRenderer.transform.position,
                                            previewRenderer.transform.rotation);

        ///
        gameObject.GetComponent<ConnectableTile>().Create(tileType, rotation);

        Neighbors neighbors = CheckNeighbors(gridPosition, selectedObject);

        if (!neighbors.Down.free)
        {
            // Set down
            gameObject.GetComponent<ConnectableTile>().SetNeighor(0, placedGameObjects[neighbors.Down.placedObjectIndex].GetComponent<ConnectableTile>());
            placedGameObjects[neighbors.Down.placedObjectIndex].GetComponent<ConnectableTile>().SetNeighor(2, gameObject.GetComponent<ConnectableTile>());
            placedGameObjects[neighbors.Down.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh();
        }

        if (!neighbors.Right.free)
        {
            // Set Right
            gameObject.GetComponent<ConnectableTile>().SetNeighor(1, placedGameObjects[neighbors.Right.placedObjectIndex].GetComponent<ConnectableTile>());
            placedGameObjects[neighbors.Right.placedObjectIndex].GetComponent<ConnectableTile>().SetNeighor(3, gameObject.GetComponent<ConnectableTile>());
            placedGameObjects[neighbors.Right.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh();
        }

        if (!neighbors.Up.free)
        {
            // Set up
            gameObject.GetComponent<ConnectableTile>().SetNeighor(2, placedGameObjects[neighbors.Up.placedObjectIndex].GetComponent<ConnectableTile>());
            placedGameObjects[neighbors.Up.placedObjectIndex].GetComponent<ConnectableTile>().SetNeighor(0, gameObject.GetComponent<ConnectableTile>());
            placedGameObjects[neighbors.Up.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh();
        }

        if (!neighbors.Left.free)
        {
            // Set Left
            gameObject.GetComponent<ConnectableTile>().SetNeighor(3, placedGameObjects[neighbors.Left.placedObjectIndex].GetComponent<ConnectableTile>());
            placedGameObjects[neighbors.Left.placedObjectIndex].GetComponent<ConnectableTile>().SetNeighor(1, gameObject.GetComponent<ConnectableTile>());
            placedGameObjects[neighbors.Left.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh();
        }

        gameObject.GetComponent<ConnectableTile>().UpdateMesh();


        placedGameObjects.Add(gameObject);
        ///

        GridData selectedData = furnitureData;

        selectedData.AddObjectAt(gridPosition, selectedObject.Size, selectedObject.ID, placedGameObjects.Count - 1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <param name="selectedObject"></param>
    /// <returns></returns>
    private bool CheckPlacementValidity(Vector3Int gridPosition, InventoryItemData_Placeable selectedObject)
    {
        GridData selectedData = furnitureData;

        return selectedData.CanPlaceObjectAt(gridPosition, selectedObject.Size);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <param name="selectedObject"></param>
    /// <returns></returns>
    private Neighbors CheckNeighbors(Vector3Int gridPosition, InventoryItemData_Placeable selectedObject)
    {
        GridData selectedData = furnitureData;

        return selectedData.GetNeighborsFree(gridPosition, selectedObject.ID);
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopPlacement()
    {
        selectedObject = null;
        if (gridVisualization) gridVisualization.enabled = false;
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnUpdate -= OnUpdateNormalMesh;
        inputManager.OnRotate -= Rotate;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnUpdateNormalMesh()
    {
        if (!IsActive) return;

        if (!selectedObject) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        if (mousePosition == Vector3.zero) return;

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (gridPosition == lastGridPosition) return;
        lastGridPosition = gridPosition;
        PositionChanged?.Invoke(lastGridPosition);

        previewRenderer.GetComponent<MeshRenderer>().material = CheckPlacementValidity(gridPosition, selectedObject) ? materials[0] : materials[1];

        cellIndicator.transform.position = new Vector3(grid.GetCellCenterWorld(gridPosition).x, cellIndicator.transform.position.y, grid.GetCellCenterWorld(gridPosition).z);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnUpdateConnectedType()
    {
        if (!IsActive) return;

        if (!selectedObject) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        if (mousePosition == Vector3.zero) return;

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (gridPosition == lastGridPosition) return;
        lastGridPosition = gridPosition;

        foreach (int i in ObjectIndexToReset)
        {
            placedGameObjects[i].GetComponent<ConnectableTile>().ResetMesh();
        }

        ObjectIndexToReset.Clear();

        bool valid = CheckPlacementValidity(gridPosition, selectedObject);
        previewRenderer.GetComponent<MeshRenderer>().material = valid ? materials[0] : materials[1];

        if (!valid) return;

        Neighbors neighbors = CheckNeighbors(gridPosition, selectedObject);

        //if (neighbors.Up.free           // Post
        //    && neighbors.Down.free
        //    && neighbors.Left.free
        //    && neighbors.Right.free)
        //{
            rotation = Quaternion.Euler(0, 0, 0);
            tileType = ConnectableTile.TileType.Post;
            previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.Post).GetComponent<MeshFilter>().sharedMesh;
        //}
        //else if (neighbors.Up.free       // Right only
        //    && neighbors.Down.free
        //    && neighbors.Left.free
        //    && !neighbors.Right.free)
        //{
        //    tileType = ConnectableTile.TileType.End;
        //    rotation = Quaternion.Euler(0, -90, 0);
        //    NeedToReset = true;
        //    ObjectIndexToReset.Add(neighbors.Right.placedObjectIndex);
        //    previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.End).GetComponent<MeshFilter>().sharedMesh;
        //    previewRenderer.transform.SetPositionAndRotation(previewRenderer.transform.position, Quaternion.Euler(0, -90, 0));
        //    //placedGameObjects[neighbors.Right.placedObjectIndex].GetComponent<ConnectableTile>().SetNeighor(3, new ConnectableTile(tileType, rotation));
        //}
        //else if (neighbors.Up.free       // Left only
        //    && neighbors.Down.free
        //    && !neighbors.Left.free
        //    && neighbors.Right.free)
        //{
        //    tileType = ConnectableTile.TileType.End;
        //    rotation = Quaternion.Euler(0, 90, 0);
        //    NeedToReset = true;
        //    ObjectIndexToReset.Add(neighbors.Left.placedObjectIndex);
        //    previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.End).GetComponent<MeshFilter>().sharedMesh;
        //    previewRenderer.transform.SetPositionAndRotation(previewRenderer.transform.position, Quaternion.Euler(0, 90, 0));
        //    //placedGameObjects[neighbors.Left.placedObjectIndex].GetComponent<ConnectableTile>().SetNeighor(1s, new ConnectableTile(tileType, rotation));
        //}
        //else if (!neighbors.Up.free       // Top only
        //    && neighbors.Down.free
        //    && neighbors.Left.free
        //    && neighbors.Right.free)
        //{
        //    tileType = ConnectableTile.TileType.End;
        //    rotation = Quaternion.Euler(0, 180, 0);
        //    NeedToReset = true;
        //    ObjectIndexToReset.Add(neighbors.Up.placedObjectIndex);
        //    previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.End).GetComponent<MeshFilter>().sharedMesh;
        //    previewRenderer.transform.SetPositionAndRotation(previewRenderer.transform.position, Quaternion.Euler(0, 180, 0));
        //    //placedGameObjects[neighbors.Up.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh(ConnectableTile.TileType.End, Quaternion.Euler(0, 0, 0));
        //}
        //else if (neighbors.Up.free       // Down only
        //    && !neighbors.Down.free
        //    && neighbors.Left.free
        //    && neighbors.Right.free)
        //{
        //    rotation = Quaternion.Euler(0, 0, 0);
        //    tileType = ConnectableTile.TileType.End;
        //    NeedToReset = true;
        //    ObjectIndexToReset.Add(neighbors.Down.placedObjectIndex);
        //    previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.End).GetComponent<MeshFilter>().sharedMesh;
        //    previewRenderer.transform.SetPositionAndRotation(previewRenderer.transform.position, Quaternion.Euler(0, 0, 0));
        //    //placedGameObjects[neighbors.Down.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh(ConnectableTile.TileType.End, Quaternion.Euler(0, 180, 0));
        //}
        //else if (!neighbors.Up.free       // Vertical
        //   && !neighbors.Down.free
        //   && neighbors.Left.free
        //   && neighbors.Right.free)
        //{
        //    tileType = ConnectableTile.TileType.Straight;
        //    rotation = Quaternion.Euler(0, 0, 0);
        //    NeedToReset = true;
        //    ObjectIndexToReset.Add(neighbors.Up.placedObjectIndex);
        //    ObjectIndexToReset.Add(neighbors.Down.placedObjectIndex);
        //    previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.Straight).GetComponent<MeshFilter>().sharedMesh;
        //    previewRenderer.transform.SetPositionAndRotation(previewRenderer.transform.position, Quaternion.Euler(0, 0, 0));
        //    placedGameObjects[neighbors.Down.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh(ConnectableTile.TileType.End, Quaternion.Euler(0, 180, 0));
        //    //placedGameObjects[neighbors.Up.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh(ConnectableTile.TileType.End, Quaternion.Euler(0, 0, 0));
        //}
        //else if (neighbors.Up.free       // Horitonzal
        //   && neighbors.Down.free
        //   && !neighbors.Left.free
        //   && !neighbors.Right.free)
        //{
        //    tileType = ConnectableTile.TileType.Straight;
        //    rotation = Quaternion.Euler(0, 90, 0);
        //    NeedToReset = true;
        //    ObjectIndexToReset.Add(neighbors.Right.placedObjectIndex);
        //    ObjectIndexToReset.Add(neighbors.Left.placedObjectIndex);
        //    previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.Straight).GetComponent<MeshFilter>().sharedMesh;
        //    previewRenderer.transform.SetPositionAndRotation(previewRenderer.transform.position, Quaternion.Euler(0, 90, 0));
        //    placedGameObjects[neighbors.Right.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh(ConnectableTile.TileType.End, Quaternion.Euler(0, -90, 0));
        //    //placedGameObjects[neighbors.Left.placedObjectIndex].GetComponent<ConnectableTile>().UpdateMesh(ConnectableTile.TileType.End, Quaternion.Euler(0, 90, 0));
        //}
        //else
        //{
        //    previewRenderer.GetComponent<MeshFilter>().mesh = connectedTile.GetGameObjectAt(ConnectableTile.TileType.Cross).GetComponent<MeshFilter>().sharedMesh;
        //}

        cellIndicator.transform.position = new Vector3(grid.GetCellCenterWorld(gridPosition).x, cellIndicator.transform.position.y, grid.GetCellCenterWorld(gridPosition).z);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Rotate()
    {
        if (!selectedObject) return;

        previewRenderer.transform.Rotate(0, 90, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="active"></param>
    public void SetActive(bool active)
    {
        IsActive = active;

        if (!IsActive) StopPlacement(); 
    }
}
