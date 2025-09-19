using System;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private StateManager stateManager;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private GameObject optionsPrefab;

    private string name;
    private DialogueAsset dialogueAsset;
    private int dialogueIndex = 0;
    private NPCRelationship speaker;
    private bool MakingChoice = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dialogue"></param>
    /// <param name="name"></param>
    public void ShowDialogue(DialogueAsset dialogue, string name, NPCRelationship speaker)
    {
        stateManager.SetState(StateManager.GameState.Dialogue);

        dialoguePanel.SetActive(true);
        dialogueIndex = 0;
        this.name = nameText.text = name;
        dialogueText.text = dialogue.dialogues[dialogueIndex].dialogue;

        dialogueAsset = dialogue;

        this.speaker = speaker;
    }

    /// <summary>
    /// 
    /// </summary>
    public void NextDialogue(int newIndex = -1)
    {
        if (dialogueAsset == null || MakingChoice) return;

        // Change index
        if (newIndex == -1)
        {
            // Affect relationship
            if (dialogueAsset.dialogues[dialogueIndex].relationshipAffectation != 0) speaker.AddToRelationship(dialogueAsset.dialogues[dialogueIndex].relationshipAffectation);

            if (dialogueAsset.dialogues[dialogueIndex].nextDialogue != 0) dialogueIndex = dialogueAsset.dialogues[dialogueIndex].nextDialogue;
            else dialogueIndex++;
        }
        else
        {
            dialogueIndex = newIndex;
        }

        // If we're at the end of the dialogue 
        if (dialogueIndex >= dialogueAsset.dialogues.Length)
        {
            EndDialogue();
            return;
        }

        // CREATE CHOICES
        if (dialogueAsset.dialogues[dialogueIndex].options.Length != 0)
        {
            MakingChoice = true;
            int index = 0;
            foreach (Dialogue choice in dialogueAsset.dialogues[dialogueIndex].options)
            {
                DialogueChoice dialogueChoice = Instantiate(optionsPrefab, optionsParent.transform).GetComponent<DialogueChoice>();
                dialogueChoice.CreateChoice(choice.dialogue, index);
                dialogueChoice.ChoiceMade += ChoiceMade;
                index++;
            }

            dialogueText.text = "";
            nameText.text = dialogueAsset.dialogues[dialogueIndex].dialogue;
        }
        else
        {
            dialogueText.text = dialogueAsset.dialogues[dialogueIndex].dialogue;
            nameText.text = this.name;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    private void ChoiceMade(int index)
    {
        MakingChoice = false;

        if (dialogueAsset.dialogues[dialogueIndex].options[index].relationshipAffectation != 0) 
            speaker.AddToRelationship(dialogueAsset.dialogues[dialogueIndex].options[index].relationshipAffectation);

        dialogueIndex = dialogueAsset.dialogues[dialogueIndex].options[index].nextDialogue;

        DialogueChoice[] choices = optionsParent.GetComponentsInChildren<DialogueChoice>();

        foreach (DialogueChoice choice in choices)
            Destroy(choice.gameObject);

        NextDialogue(dialogueIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueIndex = 0;

        dialogueText.text = "";
        dialogueText.text = "";

        dialogueAsset = null;

        stateManager.SetState(StateManager.GameState.Gameplay);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsOpen()
    {
        return dialoguePanel.activeInHierarchy;
    }
}
