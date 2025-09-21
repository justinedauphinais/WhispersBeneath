using System;
using UnityEngine;
using UnityEngine.Events;

public class ShopKeeper : NPCInteractive
{
    [SerializeField] public string _shopName;
    [SerializeField] private ShopItemList _shopItemsHeld;
    [SerializeField] public ShopSystem _shopSystem;
    [SerializeField] public Sprite _shopSprite;

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _shopSystem = new ShopSystem(_shopItemsHeld.Items.Count, _shopItemsHeld.MaxAllowedGold, _shopItemsHeld.BuyMarkUp, _shopItemsHeld.SellMarkUp);

        foreach (var item in _shopItemsHeld.Items)
        {
            _shopSystem.AddToShop(item.ItemData, item.Amount);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interactor"></param>
    /// <param name="interactSuccessfully"></param>
    public override void ContextWheel(Interactor interactor, out bool interactSuccessfully)
    {
        string[] choices = { "Talk", "Gift", "Shop" };
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
    protected override void ChoiceMade(int index)
    {
        if (index == 2)
        {
            // Shop
            ShowShop();
            return;
        }

        base.ChoiceMade(index);
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
    public override void Interact(Interactor interactor, out bool interactSuccessfully)
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

        // Open shop menu
        ShowShop();
        interactSuccessfully = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void ShowShop()
    {
        GameObject.FindGameObjectWithTag("ShopController").GetComponent<ShopUIController>().ShowShop(this);
        GameObject.FindGameObjectWithTag("StateManager").GetComponent<StateManager>().SetState(StateManager.GameState.Shopping);
    }
}
