using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostEmployee : MonoBehaviour
{
    [SerializeField] private int _assistantCost;
    [SerializeField] private int _cashierCost;

    public int AssistantCost => _assistantCost;
    public int CashierCost => _cashierCost;
}
