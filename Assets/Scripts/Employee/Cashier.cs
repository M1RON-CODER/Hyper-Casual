using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cashier : Employee
{
    [SerializeField] private Transform _cashRegisterPosition;

    public Transform CashRegisterPosition => _cashRegisterPosition;

    #region MonoBehaviour
    private void Start()
    {
        Move(_cashRegisterPosition);
    }
    #endregion

    public void Initialize(Transform cashRegisterPosition)
    {
        _cashRegisterPosition = cashRegisterPosition;
    }
}
