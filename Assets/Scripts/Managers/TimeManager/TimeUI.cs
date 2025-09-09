using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Image image;

    private Sprite[] sprites;

    private void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("Time_Pictograms");
    }

    private void OnEnable()
    {
        TimeManager.OnMinuteChanged += UpdateTime;
        TimeManager.OnHourChanged += UpdateTime;
        TimeManager.OnHourChanged += UpdatePicture;
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChanged -= UpdateTime;
        TimeManager.OnHourChanged -= UpdateTime;
        TimeManager.OnHourChanged -= UpdatePicture;
    }

    private void UpdateTime()
    {
        timeText.text = $"{TimeManager.Hour.ToString("00")}:{TimeManager.Minute.ToString("00")}";
    }

    private void UpdatePicture()
    {
        if (TimeManager.Hour >= 5 &&  TimeManager.Hour < 8)
        {
            image.sprite = sprites[0];
        }
        else if (TimeManager.Hour >= 8 && TimeManager.Hour < 17)
        {
            image.sprite = sprites[1];
        }
        else if (TimeManager.Hour >= 17 && TimeManager.Hour < 21)
        {
            image.sprite = sprites[2];
        }
        else
        {
            image.sprite = sprites[3];
        }
    }
}
