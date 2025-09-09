using UnityEngine;

public class StateManager : MonoBehaviour
{
    public enum GameState
    {
        Gameplay, 
        Inventory,
        Pause,
        Building,
        Dialogue
    }

    [Header("Managers")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private GameObject player;

    [Header("Cameras")]
    [SerializeField] private Camera buildingCamera;
    [SerializeField] private Camera playerCamera;

    private GameState state;

    public GameState State => state;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        state = GameState.Gameplay;

        SetState(GameState.Gameplay);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public GameState GetState() { return state; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    private void SetState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                placementSystem.SetActive(false);
                state = GameState.Gameplay;
                player.SetActive(true);
                playerCamera.enabled = true;
                buildingCamera.enabled = false;
                break;

            case GameState.Building:
                placementSystem.SetActive(true);
                state = GameState.Building;
                buildingCamera.enabled = true;
                playerCamera.enabled = false;
                player.SetActive(false);
                break;

            case GameState.Inventory:
                placementSystem.SetActive(false);
                state = GameState.Inventory;
                player.SetActive(true);
                playerCamera.enabled = true;
                buildingCamera.enabled = false;
                break;
        }

        inputManager.ChangeState(newState);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ActivateBuildingMode(InventoryItemData_Placeable item)
    {
        SetState(GameState.Building);

        placementSystem.StartPlacement(item);
    }

    // <summary>
    /// 
    /// </summary>
    public void ActivateBuildingMode(int ID)
    {
        SetState(GameState.Building);

        placementSystem.StartPlacement(ID);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ActivateGameplayMode()
    {
        SetState(GameState.Gameplay);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ToggleBuildingMode()
    {
        if (state == GameState.Building) ActivateGameplayMode();
        else ActivateBuildingMode(1);
    }

    public void ActivateInventoryMode()
    {
        SetState(GameState.Inventory);
    }
}