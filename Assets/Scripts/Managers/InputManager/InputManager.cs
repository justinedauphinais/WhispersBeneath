using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private StateManager stateManager;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private ShopUIController shopUIController;
    [SerializeField] private JournalUIController journalUIController;
    [SerializeField] private Interactor player;

    [Header("Inventory")]
    [SerializeField] private PlayerInventoryHolder playerInventoryHolder;
    [SerializeField] private StaticInventoryDisplay hotbar;

    [Header("Others")]
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private float holdThreshold = 0.35f;

    [Header("Debug")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] public int DebugBuildItem = 4;

    private Vector3 lastPosition;

    /// Building system variables
    public event Action OnClicked, OnExit, OnUpdate, OnRotate;

    public LayerMask PlacementMask => placementMask;
    public Vector3 LastPosition => lastPosition;

    private float pressStartTime;

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        // Player movement is in the file PlayerMovement.cs - It is only activated 
        switch (stateManager.State)
        {
            case StateManager.GameState.Gameplay:
                if (pressStartTime != 0)
                {
                    float heldTime = Time.time - pressStartTime;

                    if (heldTime >= holdThreshold)
                    {
                        pressStartTime = 0;
                        if (player.ContextWheel())
                            stateManager.ActivateDialogueMode();
                    }
                }

                if (Keyboard.current.bKey.wasPressedThisFrame)
                {
                    playerInventoryHolder.RequestInventory();
                    stateManager.ActivateInventoryMode();
                }
                else if (Mouse.current.scroll.up.value > 0)
                    hotbar.ScrollUp();
                else if (Mouse.current.scroll.down.value > 0)
                    hotbar.ScrollDown();
                else if (Keyboard.current.eKey.wasPressedThisFrame)
                    hotbar.UseItem();
                else if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
                    hotbar.SetIndex(0);
                else if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
                    hotbar.SetIndex(1);
                else if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
                    hotbar.SetIndex(2);
                else if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame)
                    hotbar.SetIndex(3);
                else if (Keyboard.current.digit5Key.wasPressedThisFrame || Keyboard.current.numpad5Key.wasPressedThisFrame)
                    hotbar.SetIndex(4);
                else if (Keyboard.current.digit6Key.wasPressedThisFrame || Keyboard.current.numpad6Key.wasPressedThisFrame)
                    hotbar.SetIndex(5);
                else if (Keyboard.current.digit7Key.wasPressedThisFrame || Keyboard.current.numpad7Key.wasPressedThisFrame)
                    hotbar.SetIndex(6);
                else if (Keyboard.current.digit8Key.wasPressedThisFrame || Keyboard.current.numpad8Key.wasPressedThisFrame)
                    hotbar.SetIndex(7);
                else if (Keyboard.current.digit9Key.wasPressedThisFrame || Keyboard.current.numpad9Key.wasPressedThisFrame)
                    hotbar.SetIndex(8);
                else if (Keyboard.current.digit0Key.wasPressedThisFrame || Keyboard.current.numpad0Key.wasPressedThisFrame)
                    hotbar.SetIndex(9);

                if (Keyboard.current.tKey.wasPressedThisFrame)
                    stateManager.ActivateBuildingMode(DebugBuildItem);

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    pressStartTime = Time.time;
                }

                if (Keyboard.current.eKey.wasReleasedThisFrame)
                {
                    float heldTime = Time.time - pressStartTime;
                    pressStartTime = 0;

                    if (heldTime < holdThreshold)
                        player.Interact();
                    else
                    {
                        if (player.ContextWheel())
                            stateManager.ActivateDialogueMode();
                    }
                }

                if (Keyboard.current.pKey.wasPressedThisFrame)
                {
                    InventoryItemData inventoryItemData = itemDatabase.GetItem(5);
                    playerInventoryHolder.AddToInventory(inventoryItemData, 1);
                }

                if (Keyboard.current.cKey.wasPressedThisFrame)
                {
                    inventoryUIController.DisplayCalendar();
                }

                if (Keyboard.current.jKey.wasPressedThisFrame)
                {
                    journalUIController.DisplaySelf();
                }

                return;

            case StateManager.GameState.Building:
                OnUpdate?.Invoke();

                if (Input.GetMouseButtonDown(0))
                    OnClicked?.Invoke();
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OnExit?.Invoke();
                    stateManager.ActivateGameplayMode();
                }
                else if (Input.GetKeyDown(KeyCode.R))
                    OnRotate?.Invoke();
                
                return;

            case StateManager.GameState.Inventory:
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    inventoryUIController.CloseInventories();
                    stateManager.ActivateGameplayMode();
                }
                else if (Mouse.current.middleButton.wasPressedThisFrame)
                {
                    inventoryUIController.MiddleButton();
                }
                return;

            case StateManager.GameState.Shopping:
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    inventoryUIController.CloseInventories();
                    stateManager.ActivateGameplayMode();
                }
                else if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    shopUIController.RightClick();
                } 
                return;

            case StateManager.GameState.Dialogue:
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    dialogueSystem.NextDialogue();
                }

                return;

            case StateManager.GameState.Pause:
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    inventoryUIController.CloseInventories();
                    stateManager.ActivateGameplayMode();
                }
                return;

            default:
                return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ChangeState(StateManager.GameState state)
    {
        switch (state)
        {
            case StateManager.GameState.Gameplay:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                player.gameObject.GetComponent<Player>().allowMove = true;

                return;

            default:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                player.gameObject.GetComponent<Player>().allowMove = false;

                return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsPointedOverUI() => EventSystem.current.IsPointerOverGameObject();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }
}
