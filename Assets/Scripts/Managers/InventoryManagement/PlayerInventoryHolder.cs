using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInventoryHolder : InventoryHolder
{
    public static UnityAction OnPlayerInventoryChanged;

    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        //SaveGameManager.data.playerInventory = new InventorySaveData(primaryInventorySystem);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    //protected override void LoadInventory(SaveData data)
    //{
    //    if (data.playerInventory.InvSystem != null)
    //    {
    //        this.primaryInventorySystem = data.playerInventory.InvSystem;
    //        OnPlayerInventoryChanged?.Invoke();
    //    }
    //}

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame) OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool AddToInventory(InventoryItemData data, int amount)
    {
        if (primaryInventorySystem.AddToInventory(data, amount))
        {
            return true;
        }

        return false;
    }
}
