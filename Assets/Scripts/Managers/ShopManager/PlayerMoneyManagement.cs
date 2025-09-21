using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMoneyManagement : MonoBehaviour
{
    [SerializeField] private MoneyAmount moneyAmount;

    public Action<MoneyAmount> GoldAmountChanged;

    public MoneyAmount MoneyAmount => moneyAmount;

    public void Start()
    {
        GoldAmountChanged?.Invoke(moneyAmount);
    }

    public void AddMoney(MoneyAmount moneyAmount)
    {
        this.moneyAmount += moneyAmount;
    }
}