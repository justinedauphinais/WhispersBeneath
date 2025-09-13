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
                returnVal.Add(position + new Vector3Int(x, 0, y));
            }
        }

        return returnVal;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Neighbors GetNeighborsFree(Vector3Int position, int ID)
    {
        Neighbors neighbors = new Neighbors();

        // Check up
        Vector3Int posUp = new Vector3Int(position.x, position.y, position.z + 1);
        if (placedObjects.ContainsKey(posUp))
            if (placedObjects[posUp].ID == ID)
                neighbors.Up = new Neighbors.PositionFree(posUp, false, placedObjects[posUp].PlacedObjectIndex);
            else
                neighbors.Up = new Neighbors.PositionFree(posUp, true);
        else
            neighbors.Up = new Neighbors.PositionFree(posUp, true);

        // Check down
        Vector3Int posDown = new Vector3Int(position.x, position.y, position.z - 1);
        if (placedObjects.ContainsKey(posDown))
            if (placedObjects[posDown].ID == ID)
                neighbors.Down = new Neighbors.PositionFree(posDown, false, placedObjects[posDown].PlacedObjectIndex);
            else
                neighbors.Down = new Neighbors.PositionFree(posDown, true);
        else
            neighbors.Down = new Neighbors.PositionFree(posDown, true);

        // Check left
        Vector3Int posLeft = new Vector3Int(position.x - 1, position.y, position.z);
        if (placedObjects.ContainsKey(posLeft))
            if (placedObjects[posLeft].ID == ID)
                neighbors.Left = new Neighbors.PositionFree(posLeft, false, placedObjects[posLeft].PlacedObjectIndex);
            else
                neighbors.Left = new Neighbors.PositionFree(posLeft, true);
        else
            neighbors.Left = new Neighbors.PositionFree(posLeft, true);

        // Check right
        Vector3Int posRight = new Vector3Int(position.x + 1, position.y, position.z);
        if (placedObjects.ContainsKey(posRight))
            if (placedObjects[posRight].ID == ID)
                neighbors.Right = new Neighbors.PositionFree(posRight, false, placedObjects[posRight].PlacedObjectIndex);
            else
                neighbors.Right = new Neighbors.PositionFree(posRight, true);
        else
            neighbors.Right = new Neighbors.PositionFree(posRight, true);

        return neighbors;
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

public class Neighbors
{
    public PositionFree Up;
    public PositionFree Down;
    public PositionFree Left;
    public PositionFree Right;

    public Neighbors()
    {
        Up = new PositionFree();
        Down = new PositionFree();
        Left = new PositionFree();
        Right = new PositionFree();
    }

    public Neighbors(Vector3Int UpPosition, bool UpFree, Vector3Int DownPosition, bool DownFree, Vector3Int LeftPosition, bool LeftFree, Vector3Int RightPosition, bool RightFree)
    {
        Up = new PositionFree(UpPosition, UpFree);
        Down = new PositionFree(DownPosition, DownFree);
        Left = new PositionFree(LeftPosition, LeftFree);
        Right = new PositionFree(RightPosition, RightFree);
    }

    public class PositionFree
    {
        public Vector3Int position;
        public bool free;
        public int placedObjectIndex;

        public PositionFree()
        {
            position = new Vector3Int();
            free = true;
            placedObjectIndex = -1;
        }

        public PositionFree(Vector3Int position, bool free)
        {
            this.position = position;
            this.free = free;
            this.placedObjectIndex = -1;
        }

        public PositionFree(Vector3Int position, bool free, int placedObjectIndex)
        {
            this.position = position;
            this.free = free;
            this.placedObjectIndex = placedObjectIndex;
        }
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