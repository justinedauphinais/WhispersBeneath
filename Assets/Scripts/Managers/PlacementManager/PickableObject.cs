using UnityEngine;
using UnityEngine.Events;

public class PickableObject : MonoBehaviour
{
    [SerializeField] public InventoryItemData_Placeable inventoryItemData;
    [SerializeField] public Vector3Int positionOnGrid = Vector3Int.zero;

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    //public void EndInteraction()
    //{
    //    // 
    //}

    //public void Interact(Interactor interactor, out bool interactSuccessfully)
    //{
    //    // Pick up
    //    interactSuccessfully = true;
    //    OnInteractionComplete?.Invoke(this);
    //}
}
