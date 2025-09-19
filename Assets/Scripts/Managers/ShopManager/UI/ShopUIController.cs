using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shopName;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI bronze, silver, gold;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject panel;
    private ShopSlot_UI[] slots;

    private void Awake()
    {
        slots = inventory.GetComponentsInChildren<ShopSlot_UI>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shop"></param>
    public void ShowShop(ShopKeeper shop)
    {
        shopName.text = shop._shopName;
        characterSprite.sprite = shop._shopSprite;
        bronze.text = shop._shopSystem._availableGold.bronzeAmount.ToString();
        silver.text = shop._shopSystem._availableGold.silverAmount.ToString();
        gold.text = shop._shopSystem._availableGold.goldAmount.ToString();

        int index = 0;
        foreach (ShopSlot item in shop._shopSystem._shopInventory)
        {
            slots[index].UpdateUISlot(item);
            index++;
        }

        panel.SetActive(true);

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

        panel.SetActive(false);
    }
}
