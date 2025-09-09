using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    //private MeshRenderer previewRenderer;

    private List<GameObject> placedGameObjects = new List<GameObject>();

    private MeshRenderer gridVisualization;

    private bool IsActive = false;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        floorData = new GridData();
        furnitureData = new GridData();
        //previewRenderer = cellIndicator.GetComponent<MeshRenderer>();
        gridVisualization = grid.GetComponent<MeshRenderer>();

        StopPlacement();
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
        gridVisualization.enabled = true;
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
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
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlaceStructure()
    {
        if (inputManager.IsPointedOverUI()) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (!CheckPlacementValidity(gridPosition, selectedObject))
        {
            return;
        }

        GameObject gameObject = Instantiate(selectedObject.ItemPrefab, new Vector3(grid.GetCellCenterWorld(gridPosition).x, 0f, grid.GetCellCenterWorld(gridPosition).z), new Quaternion(0f, 0f, 0f, 0f));

        placedGameObjects.Add(gameObject);

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
    public void StopPlacement()
    {
        selectedObject = null;
        if (gridVisualization) gridVisualization.enabled = false;
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (!IsActive) return;

        if (Input.GetKeyDown(KeyCode.Q)) StartPlacement(1);

        if (!selectedObject) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        if (mousePosition == Vector3.zero) return;

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        //previewRenderer.material.color = CheckPlacementValidity(gridPosition, selectedObject) ? Color.white : Color.red;

        cellIndicator.transform.position = new Vector3(grid.GetCellCenterWorld(gridPosition).x, cellIndicator.transform.position.y, grid.GetCellCenterWorld(gridPosition).z);
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
