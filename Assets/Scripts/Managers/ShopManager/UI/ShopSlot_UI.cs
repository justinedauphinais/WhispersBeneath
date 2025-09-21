using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot_UI : MonoBehaviour
{
    [Header("Image & Count")]
    [SerializeField] protected Image itemSprite;
    [SerializeField] protected Image moneySprite;
    [SerializeField] protected TextMeshProUGUI itemCount;
    [SerializeField] protected TextMeshProUGUI price;
    [SerializeField] protected Transform money;

    [Header("Inventory Slot Information")]
    [SerializeField] protected ShopSlot assignedInventorySlot;

    protected Button button;

    public ShopSlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; protected set; }

    public bool CanAffordSingle;

    /// <summary>
    /// Default values.
    /// </summary>
    protected void Awake()
    {
        ClearSlot();

        itemSprite.preserveAspect = true;

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }

    /// <summary>
    /// Initialisation with a slot.
    /// </summary>
    /// <param name="slot">Slot</param>
    public void Init(ShopSlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    /// <summary>
    /// Update whenever backend change.
    /// </summary>
    /// <param name="slot">New data</param>
    public virtual void UpdateUISlot(ShopSlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.Icon;
            itemSprite.color = Color.white;

            if (slot.StackSize > 1) itemCount.text = slot.StackSize.ToString();
            else itemCount.text = "";

            money.gameObject.SetActive(true);
            price.text = slot.ItemData.Price.ToString("00");

            assignedInventorySlot = slot;

            //AssignedInventorySlot.OnInventorySlotChanged?.Invoke(AssignedInventorySlot);
        }
        else
        {
            ClearSlot();
        }
    }

    /// <summary>
    /// Empty update.
    /// </summary>
    public virtual void UpdateUISlot()
    {
        if (assignedInventorySlot != null) UpdateUISlot(assignedInventorySlot);
    }

    /// <summary>
    /// Clear the data.
    /// </summary>
    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();

        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
        money.gameObject.SetActive(false);
        //AssignedInventorySlot.OnInventorySlotChanged?.Invoke(AssignedInventorySlot);
    }

    /// <summary>
    /// Triggered when clicked.
    /// </summary>
    public void OnUISlotClick()
    {
        //ParentDisplay?.SlotClicked(this, false);
    }

    /// <summary>
    /// Triggered when clicked.
    /// </summary>
    public void OnUISlotClick(bool middleMouse = false)
    {
        //ParentDisplay?.SlotClicked(this, middleMouse);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    public void SetSprite(Sprite sprite)
    {
        this.GetComponent<Image>().sprite = sprite;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="can"></param>
    public void CanAfford(bool can)
    {
        switch (can)
        {
            case true:
                price.color = Color.white;
                moneySprite.color = new Color(1f, 1f, 1f, 1f);
                CanAffordSingle = true;
                break;

            case false:
                price.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
                moneySprite.color = new Color(1f, 1f, 1f, 0.5f);
                CanAffordSingle = false;
                break;
        }
    }
}
