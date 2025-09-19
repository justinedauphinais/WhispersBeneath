using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;
    public static Action OnDayChanged;
    public static Action OnWeekChanged;
    public static Action OnMonthChanged;
    public static Action OnYearChanged;
    public static Action OnNightTriggererd;
    public static Action OnDayTriggererd;

    [SerializeField] public Transform directionalLight;

    public static int Minute {  get; private set; }
    public static int Hour { get; private set; }
    public static int Day { get; private set; }
    public static int Week { get; private set; }
    public static int Month { get; private set; }
    public static int Year { get; private set; }

    public static bool IsNight { get; private set; }

    private float timer;

    private bool isPaused;

    private float rotationSpeed;
    public float dayLengthMinutes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Minute = 00;
        Hour = 9;
        Day = 1;
        Week = 1;
        Month = 1;
        Year = 1;
        timer = dayLengthMinutes * 60 / (1440);
        rotationSpeed = 360 / dayLengthMinutes / 60;

        OnHourChanged?.Invoke();

        directionalLight.transform.SetPositionAndRotation(directionalLight.position, new Quaternion((float)(15 * Hour - 75 + 0.25 * Minute), 0f, 0f, 0f));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pause"></param>
    public void PauseTime(bool pause)
    {
        isPaused = pause;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused) return;

        directionalLight.Rotate(new Vector3(1, 0, 0) * rotationSpeed * Time.deltaTime);

        timer -= Time.deltaTime;

        // Minute advance
        if (timer <= 0)
        {
            AdvanceMinute();

            timer = dayLengthMinutes * 60 / (1440);
        }
    }

    private void AdvanceMinute()
    {
        Minute++;
        OnMinuteChanged?.Invoke();

        if (Minute < 60) return;

        Minute = 0;
        AdvanceHour();
    }

    private void AdvanceHour()
    {
        Hour++;

        if (Hour < 24) return;

        OnHourChanged?.Invoke();

        if (Hour < 5 || Hour >= 21)
        {
            if (!IsNight) OnNightTriggererd?.Invoke();

            IsNight = true;
        }
        else if (Hour == 5)
        {
            if (IsNight) OnDayTriggererd?.Invoke();

            IsNight = false;
        }
        else
            IsNight = false;

        Hour = 0;
        AdvanceDay();
    }

    private void AdvanceDay()
    {
        Day++;
        OnDayChanged?.Invoke();

        if (Day < 8) return;

        Day = 0;
        AdvanceWeek();
    }

    private void AdvanceWeek()
    {
        Week++;
        OnWeekChanged?.Invoke();

        if (Week < 5) return;

        Week = 0;
        AdvanceMonth();
    }

    private void AdvanceMonth()
    {
        Month++;
        OnMonthChanged?.Invoke();

        if (Month < 5) return;

        Month = 0;
        AdvanceYear();
    }

    private void AdvanceYear()
    {
        Year++;
        OnYearChanged?.Invoke();
    }

}

public enum WeekDay
{
    Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
}

public enum Month
{
    Spring, Summer, Autumn, Winter
}