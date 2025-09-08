using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    [SerializeField] public Transform directionalLight;

    public static int Minute {  get; private set; }
    public static int Hour { get; private set; }

    private float timer;

    private float rotationSpeed;
    public float dayLengthMinutes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Minute = 0;
        Hour = 5;
        timer = dayLengthMinutes * 60 / (1440);
        rotationSpeed = 360 / dayLengthMinutes / 60;
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
                }

                OnHourChanged?.Invoke();
            }

            timer = dayLengthMinutes * 60 / (1440);
        }
    }
}
