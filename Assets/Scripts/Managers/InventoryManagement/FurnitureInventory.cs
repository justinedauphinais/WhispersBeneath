using System;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof(UniqueID))]
public class FurnitureInventory : InventoryHolder, IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        var chestSaveData = new InventorySaveData(primaryInventorySystem, transform.position);

        //SaveGameManager.data.chestDictionary.Add(GetComponent<UniqueID>().ID, chestSaveData);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    //protected override void LoadInventory(SaveData data)
    //{
    //    if (data.chestDictionary.TryGetValue(GetComponent<UniqueID>().ID, out InventorySaveData chestData))
    //    {
    //        this.primaryInventorySystem = chestData.InvSystem;
    //        this.transform.position = chestData.Position;
    //    }
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactor"></param>
    /// <param name="interactSuccessfully"></param>
    public void Interact(Interactor interactor, out bool interactSuccessfully)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem, 0);
        interactSuccessfully = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EndInteraction()
    {

    }
}