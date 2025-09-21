using System.Collections;
using UnityEngine;

/// <summary>
/// This is a scriptable object, that defines what an item is in the game.
/// TODO: Use children to parse through placing / consume
/// </summary>
[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public int ID = -1;                             /// ID of the object - Automatically assigned by the database
    public string DisplayName;                      /// Name of the item to display
    [TextArea(4, 4)] public string Description;     /// Description of the item to display
    public Sprite Icon;                             /// Icon of the item to display
    public int MaxStackSize;                        /// Max stack of the item
    public int Price;                               /// Bronze value of the item

    public GameObject ItemPrefab;                   /// GameObject to create when placing down / putting down

    public InventoryItemDataType ItemDataType;

    /// <summary>
    /// Use the item
    /// </summary>
    public virtual bool UseItem()
    {
        Debug.Log($"Using {DisplayName}");

        return true;
    }

    /// <summary>
    /// Bought the item
    /// </summary>
    public virtual bool BoughtItem()
    {
        Debug.Log($"Bought {DisplayName}");

        return true;
    }
}

public enum InventoryItemDataType
{
    Default, 
    Decoration,
    Placeable,
    Coin,
}