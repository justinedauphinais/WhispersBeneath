using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Calendar : MonoBehaviour
{
    [SerializeField] private TMP_Text season;
    [SerializeField] private GameObject daysParent;
    [SerializeField] private GameObject view;
    [SerializeField] private Season currentSeason;

    public DayUI[] days;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        days = daysParent.GetComponentsInChildren<DayUI>();

        int i = 1;
        foreach (DayUI day in days)
        {
            day.SetDay(i);
            i++;
        }

        foreach (Events ev in currentSeason.events)
        {
            days[ev.dayIndex].AddEvent(ev.Name);
        }

        TimeManager.OnDayChanged += DayChanged;
    }

    /// <summary>
    /// 
    /// </summary>
    private void DayChanged()
    {
        int index = TimeManager.Day - 1;

        days[index].SetCurrentDay(true);

        if (index == 0) return;

        days[index - 1].SetCurrentDay(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activation"></param>
    public void ToggleActivation(bool activation)
    {
        switch (activation)
        {
            case true:
                view.SetActive(true);
                return;

            case false:
                view.SetActive(false);
                return;
        }
    }
}
