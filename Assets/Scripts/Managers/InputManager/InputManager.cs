using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private StateManager stateManager;

    private Vector3 lastPosition;

    public event Action OnClicked, OnExit;

    public LayerMask PlacementMask => placementMask;
    public Vector3 LastPosition => lastPosition;

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        else if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
        else if (Input.GetKeyDown(KeyCode.Tab)) 
            stateManager.ToggleBuildingMode();
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
