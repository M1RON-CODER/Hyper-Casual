using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EmployeePointBase))]
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class EmployeePoint : MonoBehaviour
{
    [JsonProperty("Cost")] public int Cost;
    
    [JsonIgnore] private EmployeePointBase _employeePointBase;

    public EmployeePointBase EmployeePointBase => _employeePointBase;

    #region MonoBehaviour
    #endregion

    public void Initialize(Progress progress)
    {
        _employeePointBase = GetComponent<EmployeePointBase>();
        _employeePointBase.Initialize(progress, this);
    }

    public void Withdraw(int amountOfMoney)
    {
        Cost -= amountOfMoney;
    }
}
