using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GridData
{
    SerializableDictionary<Vector3Int, PlacementData> placedObjects = new SerializableDictionary<Vector3Int, PlacementData>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="objectSize"></param>
    /// <param name="ID"></param>
    /// <param name="placedObjectIndex"></param>
    /// <exception cref="Exception"></exception>
    public void AddObjectAt(Vector3Int position,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(position, objectSize);
        PlacementData data = new PlacementData(positionsToOccupy, ID, placedObjectIndex);

        foreach (Vector3Int positionToOccupy in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(positionToOccupy))
                throw new Exception($"Dictionary already contains this cell position {positionToOccupy}.");

            placedObjects[positionToOccupy] = data;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="objectSize"></param>
    /// <returns></returns>
    private List<Vector3Int> CalculatePositions(Vector3Int position, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();

        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(position + new Vector3Int(x, y, 0));
            }
        }

        return returnVal;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="objectSize"></param>
    /// <returns></returns>
    public bool CanPlaceObjectAt(Vector3Int position, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(position, objectSize);

        foreach (Vector3Int positionToOccupy in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(positionToOccupy))
                return false;
        }

        return true;
    }
}

/// <summary>
/// 
/// </summary>
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="occupiedPositions"></param>
    /// <param name="iD"></param>
    /// <param name="placedObjectIndex"></param>
    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}