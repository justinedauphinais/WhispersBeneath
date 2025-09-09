using System;
using UnityEngine;
using UnityEngine.Events;

public class TombLogic : MonoBehaviour, IInteractable
{
    [SerializeField] private float DeteriorationRate = 1.0f;
    [SerializeField] private Mesh normalMesh;
    [SerializeField] private Mesh unkeptMesh;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject spirit;

    private MeshFilter meshFilter;

    public Color spiritColor;

    public float Maintenance = 100f;

    private Vector2Int hourToSpawn = new Vector2Int(-1, -1);

    public UnityAction<IInteractable> OnInteractionComplete { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        canvas.enabled = false;

        spiritColor = SpiritColor.RandomSpiritColor();
        spirit.GetComponent<MeshRenderer>().material.color = spiritColor;

        TimeManager.OnNightTriggererd += NightTriggered;

        spirit.SetActive(false);
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

        if (!Camera.main) return;

        canvas.transform.LookAt(transform.position + Camera.main.transform.forward);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        canvas.enabled = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
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

    /// <summary>
    /// 
    /// </summary>
    private void NightTriggered()
    {
        int beginningBound = 100 - (int)Maintenance;
        int endingBound = 100;

        float ifSpawn = UnityEngine.Random.Range(beginningBound, endingBound);

        if (ifSpawn < 70) return;

        hourToSpawn = new Vector2Int(UnityEngine.Random.Range(0, 5), UnityEngine.Random.Range(0, 60));

        TimeManager.OnHourChanged += NightHourChanged;

        NightHourChanged();
    }

    /// <summary>
    /// 
    /// </summary>
    private void NightHourChanged()
    {
        if ((TimeManager.Hour == 21 + hourToSpawn.x)
            || (TimeManager.Hour == 1 && hourToSpawn.x == 5)
            || (TimeManager.Hour == 0 && hourToSpawn.x == 4))
        {
            TimeManager.OnHourChanged -= NightHourChanged;

            if (hourToSpawn.y == 0)
                spirit.SetActive(true);
            else
            {
                TimeManager.OnMinuteChanged += NightMinuteChanged;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void NightMinuteChanged()
    {
        if (TimeManager.Minute == hourToSpawn.y)
        {
            spirit.SetActive(true);
            TimeManager.OnMinuteChanged -= NightMinuteChanged;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private class SpiritColor
    {
        public static Color RandomSpiritColor()
        {
            Color boolor = new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f), 0.75f
            );
            return boolor;
        }
    }
}
