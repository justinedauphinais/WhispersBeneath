using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// GameObject that holds an inventory (player or chest / other furniture items).
/// </summary>

[System.Serializable]
public abstract class InventoryHolder : MonoBehaviour
{
    [Header("Inventory Information")]
    [SerializeField] private int primaryInventorySize;
    [SerializeField] protected InventorySystem primaryInventorySystem;
    [SerializeField] protected int offset = 10;

    public static UnityAction<InventorySystem, int> OnDynamicInventoryDisplayRequested; // Inv system to display, amount to offset display by

    public int Offset => this.offset;

    public InventorySystem InventorySystem => this.primaryInventorySystem;

    /// <summary>
    /// Initialize the parameters
    /// </summary>
    protected virtual void Awake()
    {
        //SaveLoad.OnLoadGame += LoadInventory;

        primaryInventorySystem = new InventorySystem(primaryInventorySize);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    //protected abstract void LoadInventory(SaveData data);
}

[System.Serializable]
public struct InventorySaveData
{
    public InventorySystem InvSystem;
    public Vector3 Position;

    /// <summary>
    /// Constructor with full parameters.
    /// </summary>
    /// <param name="_invSystem"></param>
    /// <param name="_position"></param>
    public InventorySaveData(InventorySystem _invSystem, Vector3 _position)
    {
        InvSystem = _invSystem;
        Position = _position;
    }

    /// <summary>
    /// Constructor without position.
    /// </summary>
    /// <param name="_invSystem"></param>
    public InventorySaveData(InventorySystem _invSystem)
    {
        InvSystem = _invSystem;
        Position = Vector3.zero;
    }
}