using System;
using UnityEngine;
using UnityEngine.Events;

public class TombLogic : MonoBehaviour, IInteractable
{
    [Header("Grave State")]
    [Range(0f, 100f)] public float Maintenance = 100f;
    public float DecorationLevel = 0;
    [SerializeField] private float DeteriorationRate = 1.0f;
    [SerializeField] private Mesh normalMesh, unkeptMesh;
    [SerializeField] private Canvas canvas;
    
    private GraveDecorationManagement graveDecorationManagement;

    [Header("Spirit")]
    [SerializeField] private GameObject spiritPrefab;
    private Color spiritColor;

    // unrest parameters
    [SerializeField] float baseUnrestAt0Maint = 1.0f;
    [SerializeField] float baseUnrestAt100Maint = 0.05f;
    [SerializeField] float unrestGainPerMinute = 0.02f;

    private MeshFilter meshFilter;

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
        spiritPrefab.GetComponent<MeshRenderer>().material.color = spiritColor;
        spiritPrefab.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", spiritColor);

        graveDecorationManagement = GetComponent<GraveDecorationManagement>();

        TimeManager.OnNightTriggererd += NightTriggered;

        spiritPrefab.SetActive(false);
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
            else if (Maintenance < 75) meshFilter.mesh = unkeptMesh;
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
        if (Maintenance < 75)
            CleanGrave();
        else
        {
            graveDecorationManagement.OpenInventory();
        }

        interactSuccessfully = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ChoiceMade(int index)
    {
        switch (index)
        {
            case 0:
                // Clean
                CleanGrave();
                StateManager stateManager = GameObject.FindWithTag("StateManager").GetComponent<StateManager>();
                stateManager.ActivateGameplayMode();
                return;

            case 1:
                // Decorate
                graveDecorationManagement.OpenInventory();
                return;
        }
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
    /// <param name="interactor"></param>
    /// <param name="interactSuccessfully"></param>
    public void ContextWheel(Interactor interactor, out bool interactSuccessfully)
    {
        string[] choices = { "Clean", "Decorate" };
        StateManager stateManager = GameObject.FindWithTag("StateManager").GetComponent<StateManager>();
        ContextPopup context = stateManager.ShowContext("What do you want to do?", choices);
        context.ChoiceMade += ChoiceMade;

        interactSuccessfully = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void CleanGrave()
    {
        Maintenance = 100f;
        meshFilter.mesh = normalMesh;
    }

    /// <summary>
    /// 
    /// </summary>
    private void NightTriggered()
    {
        // Unrest grows as Maintenance drops. 0 = pristine, 1 = fully neglected.
        float unrest = Mathf.Clamp01(1f - (Maintenance / 100f));

        // Spawn chance curve: quadratic feels better than linear (gentle at high maintenance,
        // ramps up fast when graves are truly neglected).
        // Lerp from 5% to 90% across unrest^2.
        float spawnChance = Mathf.Lerp(0.05f, 0.90f, unrest * unrest);

        if (UnityEngine.Random.value > spawnChance)
            return; 

        float bell = (UnityEngine.Random.value + UnityEngine.Random.value + UnityEngine.Random.value) / 3f;

        int baseOffset = Mathf.RoundToInt(Mathf.Lerp(0f, 5f, bell));

        float unrestBias = Mathf.Lerp(-0.5f, 1.0f, unrest); 
        int offsetHour = Mathf.Clamp(Mathf.RoundToInt(baseOffset + unrestBias), 0, 5);

        int minute = UnityEngine.Random.Range(0, 60);

        hourToSpawn = new Vector2Int(offsetHour, minute);

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
                spiritPrefab.SetActive(true);
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
            spiritPrefab.SetActive(true);
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
                UnityEngine.Random.Range(0f, 1f), 0.33f
            );
            return boolor;
        }
    }
}
