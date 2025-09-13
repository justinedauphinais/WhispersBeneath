using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class ConnectableTile : MonoBehaviour
{
    [SerializeField] private GameObject postMesh, endMesh, cornerMesh, straightMesh, tJunctionMesh, crossMesh;
    public enum TileType { Post, End, Corner, Straight, TJunction, Cross }

    private static Dictionary<TileType, GameObject> meshMap;

    public TileType tileType;
    public TileType previousTileType;
    public Quaternion previousRotation;

    public Neighbors neighborhood;

    public class Neighbors
    {
        public ConnectableTile Up;
        public ConnectableTile Down;
        public ConnectableTile Left;
        public ConnectableTile Right;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="rotation"></param>
    public ConnectableTile(TileType tileType, Quaternion rotation)
    {
        this.name = "Awake";
        this.tileType = this.previousTileType = tileType;
        this.previousRotation = rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Awake()
    {
        Initialize(postMesh, endMesh, cornerMesh, straightMesh, tJunctionMesh, crossMesh);
        tileType = previousTileType = TileType.Post;
        neighborhood = new Neighbors();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="postMesh"></param>
    /// <param name="endMesh"></param>
    /// <param name="cornerMesh"></param>
    /// <param name="straightMesh"></param>
    /// <param name="tJunctionMesh"></param>
    /// <param name="crossMesh"></param>
    public ConnectableTile(GameObject postMesh, GameObject endMesh, GameObject cornerMesh, GameObject straightMesh, GameObject tJunctionMesh, GameObject crossMesh)
    {
        Initialize(postMesh, endMesh, cornerMesh, straightMesh, tJunctionMesh, crossMesh);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="postMesh"></param>
    /// <param name="endMesh"></param>
    /// <param name="cornerMesh"></param>
    /// <param name="straightMesh"></param>
    /// <param name="tJunctionMesh"></param>
    /// <param name="crossMesh"></param>
    public void Initialize(GameObject postMesh, GameObject endMesh, GameObject cornerMesh, GameObject straightMesh, GameObject tJunctionMesh, GameObject crossMesh)
    {
        this.postMesh = postMesh;
        this.endMesh = endMesh;
        this.cornerMesh = cornerMesh;
        this.straightMesh = straightMesh;
        this.tJunctionMesh = tJunctionMesh;
        this.crossMesh = crossMesh;

        meshMap = new Dictionary<TileType, GameObject>
        {
            { TileType.Post, postMesh },
            { TileType.End, endMesh },
            { TileType.Corner, cornerMesh },
            { TileType.Straight, straightMesh },
            { TileType.TJunction, tJunctionMesh },
            { TileType.Cross, crossMesh }
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public GameObject GetMeshAt(TileType tileType)
    {
        return meshMap[tileType];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <param name="neighbors"></param>
    public void SetNeighor(int i, ConnectableTile neighbor)
    {
        switch (i)
        {
            case 0: 
                neighborhood.Down = neighbor;
                break;

            case 1:
                neighborhood.Right = neighbor;
                break;

            case 2:
                neighborhood.Up = neighbor;
                break;

            case 3:
                neighborhood.Left = neighbor;
                break;
        }
    }

    public void UpdateMesh()
    {
        if (!neighborhood.Up && !neighborhood.Down && !neighborhood.Left && !neighborhood.Right)
        {
            // Post
            SetMesh(TileType.Post);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
        else if (!neighborhood.Up && neighborhood.Down && !neighborhood.Left && !neighborhood.Right)
        {
            // Down only
            SetMesh(TileType.End);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
        else if (!neighborhood.Up && !neighborhood.Down && !neighborhood.Left && neighborhood.Right)
        {
            // Right only
            SetMesh(TileType.End);
            transform.rotation = Quaternion.Euler(0, 270, 0);
            return;
        }
        else if (neighborhood.Up && !neighborhood.Down && !neighborhood.Left && !neighborhood.Right)
        {
            // Top only
            SetMesh(TileType.End);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            return;
        }
        else if (!neighborhood.Up && !neighborhood.Down && neighborhood.Left && !neighborhood.Right)
        {
            // Left only
            SetMesh(TileType.End);
            transform.rotation = Quaternion.Euler(0, 90, 0);
            return;
        }
        else if (neighborhood.Up && neighborhood.Down && !neighborhood.Left && !neighborhood.Right)
        {
            // Straight vertical
            SetMesh(TileType.Straight);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
        else if (!neighborhood.Up && !neighborhood.Down && neighborhood.Left && neighborhood.Right)
        {
            // Straight horizontal
            SetMesh(TileType.Straight);
            transform.rotation = Quaternion.Euler(0, 90, 0);
            return;
        }
        else if (!neighborhood.Up && neighborhood.Down && !neighborhood.Left && neighborhood.Right)
        {
            // Corner down to right
            SetMesh(TileType.Corner);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
        else if (neighborhood.Up && !neighborhood.Down && !neighborhood.Left && neighborhood.Right)
        {
            // Corner right to up
            SetMesh(TileType.Corner);
            transform.rotation = Quaternion.Euler(0, 270, 0);
            return;
        }
        else if (neighborhood.Up && !neighborhood.Down && neighborhood.Left && !neighborhood.Right)
        {
            // Corner up to left
            SetMesh(TileType.Corner);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            return;
        }
        else if (!neighborhood.Up && neighborhood.Down && neighborhood.Left && !neighborhood.Right)
        {
            // Corner left to down
            SetMesh(TileType.Corner);
            transform.rotation = Quaternion.Euler(0, 90, 0);
            return;
        }
        else if (!neighborhood.Up && neighborhood.Down && neighborhood.Left && neighborhood.Right)
        {
            // T-Junction all but Up
            SetMesh(TileType.TJunction);
            transform.rotation = Quaternion.Euler(0, 90, 0);
            return;
        }
        else if (neighborhood.Up && !neighborhood.Down && neighborhood.Left && neighborhood.Right)
        {
            // T-Junction all but down
            SetMesh(TileType.TJunction);
            transform.rotation = Quaternion.Euler(0, 270, 0);
            return;
        }
        else if (neighborhood.Up && neighborhood.Down && !neighborhood.Left && neighborhood.Right)
        {
            // T-Junction all but left
            SetMesh(TileType.TJunction);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
        else if (neighborhood.Up && neighborhood.Down && neighborhood.Left && !neighborhood.Right)
        {
            // T-Junction all but right
            SetMesh(TileType.TJunction);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            return;
        }
        else if (neighborhood.Up && neighborhood.Down && neighborhood.Left && neighborhood.Right)
        {
            // Everywhere
            SetMesh(TileType.Cross);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    public void UpdateMesh(TileType newTileType, Quaternion rotation)
    {
        UpdateMesh();

        //if (this.tileType == TileType.Post)
        //{
        //    SetMesh(newTileType);
        //    transform.rotation = rotation;
        //    return;
        //}
        //else if (this.tileType == TileType.End && newTileType == TileType.End && ((transform.rotation == Quaternion.Euler(0, 90, 0) && rotation == Quaternion.Euler(0, -90, 0)) || 
        //    (transform.rotation.eulerAngles == new Vector3(0, 270, 0) && rotation.eulerAngles == new Vector3(0, 90, 0))))
        //{
        //    SetMesh(TileType.Straight);
        //    this.tileType = TileType.Straight;
        //    transform.rotation = Quaternion.Euler(0, 90, 0);
        //    return;
        //}
        //else if (this.tileType == TileType.End && newTileType == TileType.End && ((transform.rotation == Quaternion.Euler(0, 180, 0) && rotation == Quaternion.Euler(0, 0, 0)) ||
        //    (transform.rotation.eulerAngles == new Vector3(0, 0, 0) && rotation.eulerAngles == new Vector3(0, 180, 0))))
        //{
        //    SetMesh(TileType.Straight);
        //    this.tileType = TileType.Straight;
        //    transform.rotation = Quaternion.Euler(0, 0, 0);
        //    return;
        //}
        //else if (this.tileType == TileType.End && newTileType == TileType.End && (transform.rotation == Quaternion.Euler(0, 90, 0) && rotation == Quaternion.Euler(0, 0, 0) || 
        //    transform.rotation == Quaternion.Euler(0, 0, 0) && rotation == Quaternion.Euler(0, 90, 0)))
        //{
        //    SetMesh(TileType.Corner);
        //    this.tileType = TileType.Corner;
        //    transform.rotation = Quaternion.Euler(0, 90, 0);
        //    return;
        //}
        //else if (this.tileType == TileType.End && newTileType == TileType.End && ((transform.rotation == Quaternion.Euler(0, 180, 0) && rotation.eulerAngles == new Vector3(0, 270, 0)) || 
        //    (transform.rotation == Quaternion.Euler(0, 270, 0) && rotation.eulerAngles == new Vector3(0, 180, 0))))
        //{
        //    SetMesh(TileType.Corner);
        //    this.tileType = TileType.Corner;
        //    transform.rotation = Quaternion.Euler(0, 270, 0);
        //    return;
        //}
        //else if (this.tileType == TileType.End && newTileType == TileType.End && ((transform.rotation.eulerAngles == new Vector3(0, 90, 0) && rotation.eulerAngles == new Vector3(0, 180, 0)) || 
        //    (transform.rotation.eulerAngles == new Vector3(0, 180, 0) && rotation.eulerAngles == new Vector3(0, 90, 0))))
        //{
        //    SetMesh(TileType.Corner);
        //    this.tileType = TileType.Corner;
        //    transform.rotation = Quaternion.Euler(0, 180, 0);
        //    return;
        //}
        //else if (this.tileType == TileType.End && newTileType == TileType.End && ((transform.rotation.eulerAngles == new Vector3(0, 270, 0) && rotation.eulerAngles == new Vector3(0, 0, 0)) ||
        //    transform.rotation.eulerAngles == new Vector3(0, 0, 0) && rotation.eulerAngles == new Vector3(0, 270, 0)))
        //{
        //    SetMesh(TileType.Corner);
        //    this.tileType = TileType.Corner;
        //    transform.rotation = Quaternion.Euler(0, 0, 0);
        //    return;
        //}

        Debug.Log($"{this.tileType} : {transform.rotation.eulerAngles} | {newTileType} : {rotation.eulerAngles}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    private void SetMesh(TileType tileType)
    {
        // Delete old
        previousTileType = this.tileType;
        MeshFilter[] children = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter child in children) Destroy(child.gameObject);

        // Add new 
        this.tileType = tileType;
        MeshRenderer[] newChildren = meshMap[tileType].GetComponentsInChildren<MeshRenderer>();

        foreach(MeshRenderer child in newChildren) Instantiate(child.gameObject, this.transform);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetMesh()
    {
        tileType = previousTileType;
        SetMesh(previousTileType);
        transform.rotation = previousRotation;
    }

    /// <summary>
    /// 
    /// </summary>
    public void FinalizeMesh()
    {
        previousTileType = tileType;
        previousRotation = transform.rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="rotation"></param>
    public void Create(TileType tileType, Quaternion rotation)
    {
        this.tileType = previousTileType = tileType;
        transform.rotation = previousRotation = rotation;
        SetMesh(tileType);
    }
}
