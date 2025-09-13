using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI choice;
    private int choiceIndex;

    public Action<int> ChoiceMade;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="choice_string"></param>
    /// <param name="index"></param>
    public void CreateChoice(string choice_string, int index)
    {
        choice.text = choice_string;
        choiceIndex = index;
        this.GetComponent<Button>().onClick.AddListener(Clicked);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void Clicked()
    {
        ChoiceMade?.Invoke(choiceIndex);
    }
}
