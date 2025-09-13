using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UI management of a slot
/// </summary>
public class InventorySlot_UI : MonoBehaviour
{
    [Header("Image & Count")]
    [SerializeField] protected Image itemSprite;
    [SerializeField] protected TextMeshProUGUI itemCount;

    [Header("Inventory Slot Information")]
    [SerializeField] protected InventorySlot assignedInventorySlot;

    protected Button button;

    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; protected set; }

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
    public void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    /// <summary>
    /// Update whenever backend change.
    /// </summary>
    /// <param name="slot">New data</param>
    public virtual void UpdateUISlot(InventorySlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.Icon;
            itemSprite.color = Color.white;

            if (slot.StackSize > 1) itemCount.text = slot.StackSize.ToString();
            else itemCount.text = "";

            AssignedInventorySlot.OnInventorySlotChanged?.Invoke(AssignedInventorySlot);
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
        AssignedInventorySlot.OnInventorySlotChanged?.Invoke(AssignedInventorySlot);
    }

    /// <summary>
    /// Triggered when clicked.
    /// </summary>
    public void OnUISlotClick()
    {
        ParentDisplay?.SlotClicked(this, false);
    }

    /// <summary>
    /// Triggered when clicked.
    /// </summary>
    public void OnUISlotClick(bool middleMouse = false)
    {
        ParentDisplay?.SlotClicked(this, middleMouse);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    public void SetSprite(Sprite sprite)
    {
        this.GetComponent<Image>().sprite = sprite;
    }
}
