using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;
    public static Action OnDayChanged;
    public static Action OnNightTriggererd;

    [SerializeField] public Transform directionalLight;

    public static int Minute {  get; private set; }
    public static int Hour { get; private set; }

    public static bool IsNight { get; private set; }

    private float timer;

    private float rotationSpeed;
    public float dayLengthMinutes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Minute = 55;
        Hour = 20;
        timer = dayLengthMinutes * 60 / (1440);
        rotationSpeed = 360 / dayLengthMinutes / 60;

        OnHourChanged?.Invoke();

        directionalLight.transform.SetPositionAndRotation(directionalLight.position, new Quaternion((float)(15 * Hour - 75 + 0.25 * Minute), 0f, 0f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        directionalLight.Rotate(new Vector3(1, 0, 0) * rotationSpeed * Time.deltaTime);

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Minute++;
            OnMinuteChanged?.Invoke();

            if (Minute >= 60)
            {
                Hour++;
                Minute = 0;

                if (Hour == 24)
                {
                    Hour = 0;
                    OnDayChanged?.Invoke();
                }

                OnHourChanged?.Invoke();

                if (Hour < 5 || Hour >= 21)
                {
                    if (!IsNight) OnNightTriggererd?.Invoke();

                    UnityEngine.RenderSettings.ambientIntensity = 0f;
                    UnityEngine.RenderSettings.reflectionIntensity = 0.25f;

                    IsNight = true;
                }
                else if (Hour == 5 && IsNight)
                {
                    UnityEngine.RenderSettings.ambientIntensity = 1f;
                    UnityEngine.RenderSettings.reflectionIntensity = 1f;
                }
                else
                    IsNight = false;
            }

            timer = dayLengthMinutes * 60 / (1440);
        }
    }
}
