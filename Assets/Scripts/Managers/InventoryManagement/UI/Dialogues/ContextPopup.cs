using UnityEngine;
using TMPro;
using System;

public class ContextPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private GameObject optionsPrefab;

    public Action<int> ChoiceMade;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="new_prompt"></param>
    /// <param name="dialogueChoices"></param>
    public void CreateContext(string new_prompt, string[] dialogueChoices)
    {
        this.enabled = true;
        this.prompt.text = new_prompt;

        int i = 0;
        foreach (String choice in dialogueChoices)
        {
            DialogueChoice dialogueChoice = Instantiate(optionsPrefab, optionsParent.transform).GetComponent<DialogueChoice>();
            dialogueChoice.CreateChoice(choice, i);
            dialogueChoice.ChoiceMade += ChoiceMadeAndCloseWindow;
            i++;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    private void ChoiceMadeAndCloseWindow(int index)
    {
        ChoiceMade?.Invoke(index);

        DialogueChoice[] choices = optionsParent.GetComponentsInChildren<DialogueChoice>();

        foreach (DialogueChoice choice in choices)
        {
            Destroy(choice);
        }

        this.gameObject.SetActive(false);
    }
}
