using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// "Draggable" item data by the mouse when in UI mode.
/// </summary>
public class MouseItemData : MonoBehaviour
{
    [Header("Image & Count")]
    [SerializeField] private Image ItemSprite;
    [SerializeField] private TextMeshProUGUI ItemCount;

    [Header("Inventory Slot Information")]
    public InventorySlot AssignedInventorySlot;
    public InventorySlot_UI takenFromSlot;

    private Transform PlayerTransform;

    /// <summary>
    /// Sets default values.
    /// </summary>
    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemSprite.preserveAspect = true;
        ItemCount.text = "";

        PlayerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (PlayerTransform == null)
        {
            Debug.LogError("No player with the tag Player.");
        }
    }

    /// <summary>
    /// Update the mouse slot's data.
    /// </summary>
    /// <param name="invSlot">Data and amount of the data</param>
    public void UpdateMouseSlot(InventorySlot invSlot)
    {
        AssignedInventorySlot.AssignItem(invSlot);
        ItemSprite.sprite = invSlot.ItemData.Icon;
        ItemSprite.color = Color.white;

        if (invSlot.StackSize > 1) ItemCount.text = invSlot.StackSize.ToString();
        else ItemCount.text = "";
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateMouseSlot()
    {
        if (AssignedInventorySlot.StackSize > 1) ItemCount.text = AssignedInventorySlot.StackSize.ToString();
        else ItemCount.text = "";
    }

    /// <summary>
    /// If has an item, follow mouse.
    /// If clicked, and not on UI object (inventory slot), empty hand.
    /// TODO: Add controller support. 
    /// TODO: Drop the item on the ground.
    /// </summary>
    private void Update()
    {
        if (AssignedInventorySlot == null) return;

        // If has an item, follow mouse.
        if (AssignedInventorySlot.ItemData != null)
        {
            transform.position = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            {
                // mouse position -> onmap position
                // TODO: Make better with new learnt logic
                Vector3 positionOnMap = new Vector3((transform.position.x - 956.0307f) / 31.83309f, (transform.position.y - 530f) / 17.67712f);
                Vector3 direction = (positionOnMap - PlayerTransform.position).normalized;
                GameObject gameObject = Instantiate(AssignedInventorySlot.ItemData.ItemPrefab, PlayerTransform.position + direction * 2, new Quaternion(0, 0, 0, 0));

                Collectibles collectible = gameObject.GetComponent<Collectibles>();

                if (collectible != null)
                {
                    collectible.Spawn();
                }

                if (AssignedInventorySlot.StackSize == 1) ClearSlot();
                else
                {
                    AssignedInventorySlot.AddToStack(-1);
                    UpdateMouseSlot();
                }

                return;
            }

            if (Mouse.current.scroll.up.value > 0)
            {
                if (takenFromSlot.AssignedInventorySlot.ItemData == null)
                    return;
                else if (takenFromSlot.AssignedInventorySlot.StackSize == 1)
                    takenFromSlot.AssignedInventorySlot.ClearSlot();
                else
                    takenFromSlot.AssignedInventorySlot.AddToStack(-1);

                takenFromSlot.UpdateUISlot();
                AssignedInventorySlot.AddToStack(1);
                UpdateMouseSlot();
            }
            else if (Mouse.current.scroll.down.value > 0)
            {
                if (takenFromSlot.AssignedInventorySlot.ItemData == null)
                    takenFromSlot.AssignedInventorySlot.AssignItem(new InventorySlot(AssignedInventorySlot.ItemData, 1));
                else
                    takenFromSlot.AssignedInventorySlot.AddToStack(1);

                takenFromSlot.UpdateUISlot();
                AssignedInventorySlot.AddToStack(-1);

                if (AssignedInventorySlot.StackSize == 0)
                    ClearSlot();
                else
                    UpdateMouseSlot();
            }
        }
    }

    /// <summary>
    /// Clear the slot (empty hand).
    /// </summary>
    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();
        ItemSprite.color = Color.clear;
        ItemSprite.sprite = null;
        ItemCount.text = "";
        takenFromSlot = null;
    }

    /// <summary>
    /// If the pointer is over an UI object (inventory slot).
    /// </summary>
    /// <returns></returns>
    public static bool IsPointerOverUIObject()
    {
        return PointerOverUIObject().Count > 0;
    }

    /// <summary>
    /// If the pointer is over an UI object (inventory slot).
    /// </summary>
    /// <returns></returns>
    public static List<RaycastResult> PointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results;
    }
}
