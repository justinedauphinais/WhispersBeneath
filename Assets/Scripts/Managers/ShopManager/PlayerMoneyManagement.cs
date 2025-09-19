using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMoneyManagement : MonoBehaviour
{
    [SerializeField] private MoneyAmount moneyAmount;

    public Action<MoneyAmount> GoldAmountChanged;

    public void Start()
    {
        GoldAmountChanged?.Invoke(moneyAmount);
    }
}