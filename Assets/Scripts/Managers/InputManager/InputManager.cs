using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private StateManager stateManager;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private PlayerInventoryHolder playerInventoryHolder;
    [SerializeField] private StaticInventoryDisplay hotbar;

    private Vector3 lastPosition;

    public event Action OnClicked, OnExit;

    public LayerMask PlacementMask => placementMask;
    public Vector3 LastPosition => lastPosition;

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        // Player movement is in the file PlayerMovement.cs - It is only activated 

        switch (stateManager.State)
        {
            case StateManager.GameState.Gameplay:
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
                else if (Keyboard.current.digit0Key.wasPressedThisFrame || Keyboard.current.numpad0Key.wasPressedThisFrame)
                    hotbar.SetIndex(9);

                if (Keyboard.current.tKey.wasPressedThisFrame)
                    stateManager.ActivateBuildingMode(2);

                return;

            case StateManager.GameState.Building:
                if (Input.GetMouseButtonDown(0))
                    OnClicked?.Invoke();
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OnExit?.Invoke();
                    stateManager.ActivateGameplayMode();
                }

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

                return;

            default:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

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
