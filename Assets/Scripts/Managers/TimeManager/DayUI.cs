using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp_day;
    [SerializeField] private GameObject events;
    [SerializeField] private GameObject eventPrefab;
    [SerializeField] private Image backgroundColored;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    public DayUI(string day)
    {
        tmp_day.text = day;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    public DayUI(int day)
    {
        tmp_day.text = day.ToString("00");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    public DayUI(int day, string[] eventsList)
    {
        tmp_day.text = day.ToString("00");
        AddEvents(eventsList);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    public void SetDay(int day)
    {
        tmp_day.text = day.ToString("00");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventsList"></param>
    public void AddEvents(string[] eventsList)
    {
        foreach (string ev in eventsList)
        {
            GameObject go = Instantiate(eventPrefab, events.transform);
            go.GetComponentInChildren<TMP_Text>().text = ev;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventsList"></param>
    public void AddEvent(string ev)
    {
        GameObject go = Instantiate(eventPrefab, events.transform);
        go.GetComponentInChildren<TMP_Text>().text = ev;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isCurrentday"></param>
    public void SetCurrentDay(bool isCurrentday)
    {
        switch (isCurrentday)
        {
            case true:
                backgroundColored.color = new Color(176, 189, 209);
                return;
            
            case false:
                backgroundColored.color = Color.white;
                return;
        }
    }
}