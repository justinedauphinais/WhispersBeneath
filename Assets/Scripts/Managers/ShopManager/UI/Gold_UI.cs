using TMPro;
using UnityEngine;

public class Gold_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bronze;
    [SerializeField] private TextMeshProUGUI silver;
    [SerializeField] private TextMeshProUGUI gold;

    private void Awake()
    {
        PlayerMoneyManagement playerMoneyManagement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManagement>();
        playerMoneyManagement.GoldAmountChanged += UpdateGoldTexts;
    }

    public void UpdateGoldTexts(MoneyAmount money)
    {
        bronze.text = money.bronzeAmount.ToString();
        silver.text = money.silverAmount.ToString();
        gold.text = money.goldAmount.ToString();
    }
}
