using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    [SerializeField] private GameObject _employee;

    public GameObject Employee => _employee;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            _employee.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    #endregion

    private void WithdrawCash(Player player)
    {
        
    }
}
