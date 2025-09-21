using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPCInteractive : MonoBehaviour, IInteractable
{
    [SerializeField] public string charName;
    [SerializeField] public CharacterDialogue dialogues;
    [SerializeField] public Sprite characterImage;

    protected NPCRelationship relationship;

    protected bool IsMet;

    public NPCRelationship Relationsip => relationship;

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Awake()
    {
        relationship = this.gameObject.GetComponent<NPCRelationship>();
        IsMet = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public UnityAction<IInteractable> OnInteractionComplete { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactor"></param>
    /// <param name="interactSuccessfully"></param>
    public virtual void ContextWheel(Interactor interactor, out bool interactSuccessfully)
    {
        string[] choices = { "Talk", "Gift" };
        StateManager stateManager = GameObject.FindWithTag("StateManager").GetComponent<StateManager>();
        ContextPopup context = stateManager.ShowContext("What do you want to do?", choices);
        context.ChoiceMade += ChoiceMade;

        interactSuccessfully = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual void ChoiceMade(int index)
    {
        switch (index)
        {
            case 0:
                // Talk
                TalkTo();
                return;

            case 1:
                // Gift - open menu

                return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void TalkTo()
    {
        DialogueSystem dialogueSystem = GameObject.FindGameObjectWithTag("DialogueSystem").GetComponent<DialogueSystem>();

        int level = relationship.GetRelationshipLevel(out bool TriggerEvent);

        DialogueAsset dialogueAsset = null;
        if (TriggerEvent)
            dialogueAsset = dialogues.GetEvent(level);
        else
            dialogueAsset = dialogues.GetRandomPerLevel(level);

        dialogueSystem.ShowDialogue(dialogueAsset, charName, relationship);
    }

    /// <summary>
    /// 
    /// </summary>
    public void EndInteraction()
    {
       
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactor"></param>
    /// <param name="interactSuccessfully"></param>
    public virtual void Interact(Interactor interactor, out bool interactSuccessfully)
    {
        if (!IsMet)
        {
            DialogueSystem dialogueSystemMeeting = GameObject.FindGameObjectWithTag("DialogueSystem").GetComponent<DialogueSystem>();
            DialogueAsset dialogueAssetMeeting = dialogues.GetMeetingDialogue();

            dialogueSystemMeeting.ShowDialogue(dialogueAssetMeeting, charName, relationship);

            IsMet = true;
            interactSuccessfully = false;
            return;
        }

        // Get player
        StaticInventoryDisplay inventoryDisplay = interactor.GetComponentInChildren<StaticInventoryDisplay>();
        InventorySlot inventorySlot = inventoryDisplay.GetSelectedItem();

        if (inventorySlot != null)
        {
            if (inventorySlot.ItemData != null)
            {
                this.gameObject.GetComponent<NPCRelationship>()?.AddToRelationship(10);
                interactSuccessfully = false;
                return;
            }
        }

        // Talk to
        TalkTo();

        interactSuccessfully = false;
    }
}
