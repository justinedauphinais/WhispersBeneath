using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GraveDecorationManagement : InventoryHolder
{
    [SerializeField] Transform[] transforms;
    [SerializeField] GameObject decorationsParent;

    TombLogic tombLogic;

    private SerializableDictionary<Transform, GameObject> decorationsItems = new SerializableDictionary<Transform, GameObject>();

    public static UnityAction<InventorySystem, GameObject> OnTombDisplayRequested; // Inv system to display, amount to offset display by

    /// <summary>
    /// 
    /// </summary>
    public GraveDecorationManagement()
    {
        this.primaryInventorySize = 4;
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        primaryInventorySystem = new InventorySystem(4, true, InventorySlotType.Decoration);
        primaryInventorySystem.OnInventorySlotChanged += InventorySlotChange;

        tombLogic = this.gameObject.GetComponent<TombLogic>();

        transforms = decorationsParent.GetComponentsInChildren<Transform>().Skip(1).ToArray();

        foreach (Transform t in transforms)
        {
            decorationsItems.Add(t, null);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OpenInventory()
    {
        OnTombDisplayRequested?.Invoke(primaryInventorySystem, this.gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg0"></param>
    public void InventorySlotChange(InventorySlot slot)
    {
        if (primaryInventorySystem.InventorySlots.IndexOf(slot) == -1)
            return;

        Transform index = transforms[primaryInventorySystem.InventorySlots.IndexOf(slot)];

        ClearSlots(index);

        tombLogic.DecorationLevel += ((InventoryItem_Decoration)slot.ItemData).DecorationLevel;

        GameObject gameObject = Instantiate(slot.ItemData.ItemPrefab, index);
        gameObject.transform.SetPositionAndRotation(gameObject.transform.position, Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, UnityEngine.Random.Range(0, 360), gameObject.transform.rotation.eulerAngles.z));

        decorationsItems[index] = gameObject;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    private void ClearSlots(Transform index)
    {
        if (!decorationsItems[index]) return;

        GameObject gameObject = decorationsItems[index];

        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>().Skip(1).ToArray();

        foreach (Transform t in transforms)
        {
            Destroy(t);
        }
    }
}
