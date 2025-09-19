using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Controller of the different inventory panels of the game
/// 1. Inventory Panel                  - Chests
/// 2. Player Backpack Panel            - Backpack during the use of chests
/// 3. Full Player Inventory Holder     - Full backpack view
/// 4. Static Player Inventory Holder   - Hotbar
/// </summary>
public class InventoryUIController : MonoBehaviour
{
    [Header("Inventory displays")]
    [SerializeField] private DynamicInventoryDisplay InventoryPanel;
    [SerializeField] private DynamicInventoryDisplay PlayerBackpackPanel;
    [SerializeField] private DynamicInventoryDisplay GravePanel;
    [SerializeField] private FullBackpackInventory FullPlayerInventoryHolder;
    [SerializeField] private StaticInventoryDisplay StaticPlayerInventoryHolder;
    [SerializeField] private PlayerInventoryHolder PlayerInventoryHolder;
    [SerializeField] private ShopUIController ShopUIController;
    [SerializeField] private Calendar calendar;

    [Header("HUDs")]
    [SerializeField] private RectTransform Hotbar;
    [SerializeField] private RectTransform HUD;

    private GameObject selectedGrave;

    /// <summary>
    /// Initialize the parameters
    /// </summary>
    private void Awake()
    {
        CloseInventories();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested += DisplayPlayerInventory;
        GraveDecorationManagement.OnTombDisplayRequested += DisplayGrave;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested -= DisplayPlayerInventory;
        GraveDecorationManagement.OnTombDisplayRequested -= DisplayGrave;
    }

    /// <summary>
    /// Close all inventories - When Escape Key pressed
    /// </summary>
    /// <returns></returns>
    public bool CloseInventories()
    {
        InventoryPanel.gameObject.SetActive(false);
        PlayerBackpackPanel.gameObject.SetActive(false);
        FullPlayerInventoryHolder.gameObject.SetActive(false);
        GravePanel.gameObject.SetActive(false);

        Hotbar.gameObject.SetActive(true);
        HUD.gameObject.SetActive(true);
        calendar.ToggleActivation(false);
        ShopUIController.UnshowShop();

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        return true;
    }

    /// <summary>
    /// Split stack if on UI_Item - When middle button is pressed
    /// </summary>
    /// <returns></returns>
    public bool MiddleButton()
    {
        List<RaycastResult> results = MouseItemData.PointerOverUIObject();

        InventorySlot_UI slot = null;

        foreach (RaycastResult r in results)
        {
            if (r.gameObject.GetComponent<InventorySlot_UI>() != null)
            {
                slot = r.gameObject.GetComponent<InventorySlot_UI>();
                slot.OnUISlotClick(true);

                break;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInventories();
        }
        else if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            MiddleButton();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisplayCalendar()
    {
        calendar.ToggleActivation(true);
        Hotbar.gameObject.SetActive(false);
        HUD.gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invToDisplay"></param>
    private void DisplayInventory(InventorySystem invToDisplay, int offset)
    {
        InventoryPanel.gameObject.SetActive(true);
        InventoryPanel.RefreshDynamicInventory(invToDisplay, offset);

        PlayerBackpackPanel.gameObject.SetActive(true);
        PlayerBackpackPanel.RefreshDynamicInventory(PlayerInventoryHolder.InventorySystem, PlayerInventoryHolder.Offset);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invToDisplay"></param>
    private void DisplayPlayerInventory(InventorySystem invToDisplay, int offset)
    {
        FullPlayerInventoryHolder.gameObject.SetActive(true);
        FullPlayerInventoryHolder.RefreshDynamicInventory(invToDisplay, offset);

        Hotbar.gameObject.SetActive(false);
        HUD.gameObject.SetActive(false);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void DisplayGrave(InventorySystem inventorySystem, GameObject grave)
    {
        GravePanel.gameObject.SetActive(true);
        GravePanel.RefreshDynamicInventory(inventorySystem, 0, grave);

        PlayerBackpackPanel.gameObject.SetActive(true);
        PlayerBackpackPanel.RefreshDynamicInventory(PlayerInventoryHolder.InventorySystem, PlayerInventoryHolder.Offset);

        selectedGrave = grave;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newInventory"></param>
    public void UpdateFromBackpack()
    {
        StaticPlayerInventoryHolder.UpdateAll();
    }
}
