using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    [Header("Shop UI")]
    [SerializeField] private TextMeshProUGUI shopName;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI bronze, silver, gold;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject panel;

    [Header("Buy UI")]
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI maxItems;
    [SerializeField] private TMP_InputField amount;
    [SerializeField] private Slider slider;
    [SerializeField] private Button buy;
    [SerializeField] private Button cancel;

    private ShopSlot_UI[] slots;
    private ShopKeeper currentShop;
    private ShopSlot_UI buyingItem;

    private void Awake()
    {
        slots = inventory.GetComponentsInChildren<ShopSlot_UI>();
        HideBuy();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shop"></param>
    public void ShowShop(ShopKeeper shop)
    {
        currentShop = shop;
        shopName.text = shop._shopName;
        characterSprite.sprite = shop._shopSprite;
        bronze.text = shop._shopSystem._availableGold.bronzeAmount.ToString();
        silver.text = shop._shopSystem._availableGold.silverAmount.ToString();
        gold.text = shop._shopSystem._availableGold.goldAmount.ToString();

        int index = 0;
        foreach (ShopSlot item in shop._shopSystem._shopInventory)
        {
            slots[index].UpdateUISlot(item);

            // check if can afford at 1
            slots[index].CanAfford(CheckIfCanAfford(1, item.ItemData));

            index++;
        }

        panel.SetActive(true);
        HideBuy();

        // INVENTORY
    }

    /// <summary>
    /// 
    /// </summary>
    public void UnshowShop()
    {
        foreach (ShopSlot_UI slot in slots)
        {
            slot.ClearSlot();
        }

        HideBuy();
        panel.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void RightClick()
    {
        List<RaycastResult> results = MouseItemData.PointerOverUIObject();

        ShopSlot_UI slot = null;

        foreach (RaycastResult r in results)
        {
            try
            {
                if (r.gameObject.GetComponent<ShopSlot_UI>() != null)
                {
                    slot = r.gameObject.GetComponent<ShopSlot_UI>();
                    if (slot.CanAffordSingle) ShowBuy(slot);
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowBuy(ShopSlot_UI slot)
    {
        buyingItem = slot;
        buyPanel.SetActive(true);
        itemName.text = slot.AssignedInventorySlot.ItemData.DisplayName;
        maxItems.text = slot.AssignedInventorySlot.StackSize.ToString("00");
        amount.text = "01";
        slider.maxValue = slot.AssignedInventorySlot.StackSize;
        slider.minValue = 1;
        slider.value = 1;

        cancel.onClick.AddListener(HideBuy);
        buy.onClick.AddListener(() => { Buy(slot.AssignedInventorySlot, Mathf.FloorToInt(slider.value)); });
        slider.onValueChanged.AddListener(SliderChange);
        amount.onValueChanged.AddListener(AmountInputChange);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideBuy()
    {
        itemName.text = "";
        maxItems.text = "01";
        amount.text = "01";
        slider.maxValue = 1;
        slider.minValue = 1;
        slider.value = 1;

        cancel.onClick.RemoveAllListeners();
        buy.onClick.RemoveAllListeners();
        slider.onValueChanged.RemoveAllListeners();
        amount.onValueChanged.RemoveAllListeners();
        buyPanel.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="amount"></param>
    public void Buy(ShopSlot slot, int amount)
    {
        MoneyAmount player = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMoneyManagement>().MoneyAmount;
        MoneyAmount newShop, newPlayer, loss = new MoneyAmount();
        bool success = MoneyAmount.ApplyTransaction(ref currentShop._shopSystem._availableGold, ref player, 
            buyingItem.AssignedInventorySlot.ItemData.Price * amount, true, out loss);
    }

    /// <summary>
    /// 
    /// </summary>
    private void SliderChange(float value)
    {
        amount.text = value.ToString("00");
    }

    /// <summary>
    /// 
    /// </summary>
    private void AmountInputChange(string value)
    {
        slider.value = int.Parse(value);

        if (CheckIfCanAfford(Mathf.FloorToInt(slider.value), buyingItem.AssignedInventorySlot.ItemData)) EnableBuyButton();
        else DisableBuyButton();

    }

    /// <summary>
    /// 
    /// </summary>
    private bool CheckIfCanAfford(int amount, InventoryItemData item)
    {
        MoneyAmount player = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMoneyManagement>().MoneyAmount;
        MoneyAmount newShop, newPlayer, loss, change = new MoneyAmount();
        return MoneyAmount.PreviewTransaction(currentShop._shopSystem._availableGold, player, item.Price * amount, out newShop, out newPlayer, out loss, out change);
    }

    /// <summary>
    /// 
    /// </summary>
    private void DisableBuyButton()
    {
        buy.onClick.RemoveAllListeners();
        buy.image.color = new Color(0.5482022f, 0.5787696f, 0.6679245f, 0.9f);
    }

    /// <summary>
    /// 
    /// </summary>
    private void EnableBuyButton()
    {
        buy.onClick.AddListener(() => { Buy(buyingItem.AssignedInventorySlot, Mathf.FloorToInt(slider.value)); });
        buy.image.color = new Color(0.227451f, 0.2745098f, 0.4117647f, 1f);
    }
}
