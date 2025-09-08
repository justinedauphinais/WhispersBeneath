using System;
using System.Xml;
using UnityEngine;

/// <summary>
/// Script to associate to pickable items.
/// </summary>

[RequireComponent(typeof(SphereCollider))]
//[RequireComponent(typeof(UniqueID))]
public class ItemPickUp : MonoBehaviour
{
    public float PickUpRadius = 1.0f;
    public InventoryItemData ItemData;

    private SphereCollider myCollider;

    [SerializeField] private ItemPickUpSaveData itemSaveData;
    private string id;

    /// <summary>
    /// Sets default values.
    /// </summary>
    private void Awake()
    {
        //SaveLoad.OnLoadGame += LoadGame;
        itemSaveData = new ItemPickUpSaveData(ItemData, transform.position);

        myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = true;
        myCollider.radius = PickUpRadius; 
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        //id = GetComponent<UniqueID>().ID;
        //SaveGameManager.data.activeItems.Add(id, itemSaveData);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg0"></param>
    //private void LoadGame(SaveData data)
    //{
    //    if (data.collectedItems.Contains(id)) Destroy(this.gameObject);
    //}

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        //if (SaveGameManager.data.activeItems.ContainsKey(id)) SaveGameManager.data.activeItems.Remove(id);

        //SaveLoad.OnLoadGame -= LoadGame;
    }

    /// <summary>
    /// Collider manager that destroy game object if can fit in player inventory on pick up.
    /// </summary>
    /// <param name="other">Object with which it is in collision with.</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("here");
        PlayerInventoryHolder inventory = other.transform.GetComponent<PlayerInventoryHolder>();

        if (!inventory) return;

        Debug.Log("here2");

        if (inventory.AddToInventory(ItemData, 1))
        {
            Debug.Log("here4");
            //SaveGameManager.data.collectedItems.Add(id);
            Destroy(this.gameObject);
        }
    }
}

[System.Serializable]
public struct ItemPickUpSaveData
{
    public InventoryItemData ItemData;
    public Vector3 Position;

    public ItemPickUpSaveData(InventoryItemData _data, Vector3 _position)
    {
        ItemData = _data;
        Position = _position;
    }
}