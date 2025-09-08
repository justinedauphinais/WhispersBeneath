using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Database of all the Inventory Item Data in the game.
/// </summary>
[CreateAssetMenu(menuName = "Inventory System/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<InventoryItemData> itemDatabase;

    /// <summary>
    /// Sets the item IDs for the ones found in the folder "Resources/ItemData/".
    /// To execute in the editor.
    /// </summary>
    [ContextMenu("Set IDs")]
    public void SetItemIDs()
    {
        this.itemDatabase = new List<InventoryItemData>();

        List<InventoryItemData> foundItems = Resources.LoadAll<InventoryItemData>("ItemData").OrderBy(i => i.ID).ToList();

        var hasIDInRange = foundItems.Where(i => i.ID != -1 && i.ID < foundItems.Count).OrderBy(i => i.ID).ToList();
        var hasIDNotInRange = foundItems.Where(i => i.ID != -1 && i.ID >= foundItems.Count).OrderBy(i => i.ID).ToList();
        var noID = foundItems.Where(i => i.ID <= -1).ToList();

        var index = 0;
        for (int i = 0; i < foundItems.Count; i++)
        {
            InventoryItemData itemToAdd;
            itemToAdd = hasIDInRange.Find(d => d.ID == i);

            if (itemToAdd != null)
            {
                this.itemDatabase.Add(itemToAdd);
            }
            else if (index < noID.Count)
            {
                noID[index].ID = i;
                itemToAdd = noID[index];
                index++;
                this.itemDatabase[index] = itemToAdd;
            }
        }

        foreach (InventoryItemData item in hasIDNotInRange)
        {
            this.itemDatabase.Add(item);
        }
    }

    /// <summary>
    /// Get the item with the specified ID.
    /// </summary>
    /// <param name="ID">ID of the item to get.</param>
    /// <returns>
    ///     The Inventory Item Data of the item associated to the ID. 
    ///     If non existant, returns null.
    /// </returns>
    public InventoryItemData GetItem(int ID)
    {
        return this.itemDatabase.Find(i => i.ID == ID);
    }
}
