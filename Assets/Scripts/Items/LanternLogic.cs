using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class LanternLogic : MonoBehaviour, IInteractable
{
    [Header("Scene Refs")]
    [SerializeField] private GameObject lanternGlass;
    [SerializeField] private Light pointLight;
    [SerializeField] private ParticleSystem fireflies;

    [Header("Materials")]
    [SerializeField] private Material unlitMat;
    [SerializeField] private Material litMat;

    [Header("Flicker Settings")]
    [SerializeField] private float baseIntensity = 5f;  // avg light intensity when lit
    [SerializeField] private float amplitude = 0.6f;      // how strong the flicker swings
    [SerializeField] private float speed = 3.5f;          // flicker speed
    [SerializeField] private Color emissionColor = Color.yellow; // HDR recommended in inspector
    [SerializeField] private float baseEmission = 2.0f;   // emission multiplier (HDR)

    private bool isLit = false;
    private MeshRenderer glassRenderer;
    private Coroutine flickerCo;
    private int emissionID = Shader.PropertyToID("_EmissionColor");
    private MaterialPropertyBlock mpb;
    private float seed; // per-lantern random offset

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    void Awake()
    {
        glassRenderer = lanternGlass.GetComponent<MeshRenderer>();
        mpb = new MaterialPropertyBlock();
        seed = Random.Range(0f, 1000f);
    }

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        TurnOn();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public void EndInteraction() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactor"></param>
    /// <param name="interactSuccessfully"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void Interact(Interactor interactor, out bool interactSuccessfully)
    {
        if (!isLit) TurnOn();

        interactSuccessfully = true;
        OnInteractionComplete?.Invoke(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactor"></param>
    /// <param name="interactSuccessfully"></param>
    public void ContextWheel(Interactor interactor, out bool interactSuccessfully)
    {
        interactSuccessfully = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void TurnOn()
    {
        isLit = true;

        if (pointLight != null)
        {
            pointLight.enabled = true;
            pointLight.intensity = baseIntensity;
        }

        if (fireflies != null) fireflies.gameObject.SetActive(true);

        // Option A: swap materials (simple)
        if (litMat != null) glassRenderer.material = litMat;

        //// Option B (better): drive emission via MPB without material swap
        EnableEmission(true);

        //// start flicker
        if (flickerCo != null) StopCoroutine(flickerCo);
        flickerCo = StartCoroutine(FlickerRoutine());
    }

    /// <summary>
    /// 
    /// </summary>
    private void TurnOff()
    {
        isLit = false;

        if (flickerCo != null) StopCoroutine(flickerCo);
        if (pointLight != null)
        {
            pointLight.intensity = 0f;
            pointLight.enabled = false;
        }

        if (fireflies != null) fireflies.gameObject.SetActive(false);

        // reset visuals
        SetOffVisuals();
        EnableEmission(false);
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetOffVisuals()
    {
        if (unlitMat != null) glassRenderer.material = unlitMat;

        // Clear emission via MPB
        glassRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(emissionID, Color.black);
        glassRenderer.SetPropertyBlock(mpb);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="on"></param>
    private void EnableEmission(bool on)
    {
        glassRenderer.GetPropertyBlock(mpb);
        var c = on ? (emissionColor * Mathf.LinearToGammaSpace(baseEmission)) : Color.black;
        mpb.SetColor(emissionID, c);
        glassRenderer.SetPropertyBlock(mpb);

        if (on) glassRenderer.material.EnableKeyword("_EMISSION");
        else glassRenderer.material.DisableKeyword("_EMISSION");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator FlickerRoutine()
    {
        while (isLit)
        {
            float t = Time.time * speed + seed;
            float n = (Mathf.PerlinNoise(t, t * 0.37f) * 2f) - 1f;
            float targetIntensity = Mathf.Max(0f, baseIntensity + n * amplitude);

            if (pointLight != null) pointLight.intensity = targetIntensity;

            float emissionMul = Mathf.Max(0.1f, targetIntensity / Mathf.Max(0.0001f, baseIntensity));
            glassRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(emissionID, emissionColor * Mathf.LinearToGammaSpace(baseEmission * emissionMul));
            glassRenderer.SetPropertyBlock(mpb);

            yield return null; // every frame
        }
    }
}
