using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 
/// </summary>

public class Interactor : MonoBehaviour
{
    public Transform InteractionPoint;
    public LayerMask InteractionLayer;
    public float InteractionPointRadius = 0.05f;
    public bool IsInteracting { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public void Interact()
    {
        var colliders = Physics.OverlapSphere(InteractionPoint.transform.position, InteractionPointRadius, InteractionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            var interactable = colliders[i].GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                StartInteraction(interactable);
                return;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ContextWheel()
    {
        var colliders = Physics.OverlapSphere(InteractionPoint.transform.position, InteractionPointRadius, InteractionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            var interactable = colliders[i].GetComponent<IInteractable>();

            if (interactable != null)
            {
                OpenContextWheel(interactable);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactable"></param>
    void StartInteraction(IInteractable interactable)
    {
        interactable.Interact(this, out bool interactSuccessful);
        IsInteracting = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactable"></param>
    private bool OpenContextWheel(IInteractable interactable)
    {
        interactable.ContextWheel(this, out bool interactSuccessful);
        IsInteracting = true;
        return interactSuccessful;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactable"></param>
    void EndInteraction(IInteractable interactable)
    {
        IsInteracting = false;
    }
}
