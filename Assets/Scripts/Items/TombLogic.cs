using System;
using UnityEngine;
using UnityEngine.Events;

public class TombLogic : MonoBehaviour, IInteractable
{
    [SerializeField] private float DeteriorationRate = 1.0f;
    [SerializeField] private Mesh normalMesh;
    [SerializeField] private Mesh unkeptMesh;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform playerCamera;

    private MeshFilter meshFilter;

    public float Maintenance = 100f;

    public UnityAction<IInteractable> OnInteractionComplete { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        canvas.enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        if (Maintenance != 0)
        {
            Maintenance -= (DeteriorationRate * Time.deltaTime) / 100f;

            if (Maintenance < 0) Maintenance = 0;
            else if (Maintenance < 75)
                meshFilter.mesh = unkeptMesh;
        }

        canvas.transform.LookAt(transform.position + playerCamera.forward);
    }

    public void OnTriggerEnter(Collider other)
    {
        canvas.enabled = true;
    }

    public void OnTriggerExit(Collider other)
    {
        canvas.enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactor"></param>
    /// <param name="interactSuccessfully"></param>
    public void Interact(Interactor interactor, out bool interactSuccessfully)
    {
        interactSuccessfully = true;
        Maintenance = 100f;
        meshFilter.mesh = normalMesh;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EndInteraction()
    {
        
    }
}
