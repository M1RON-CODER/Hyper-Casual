using UnityEngine;

public class EmployeePoint : ProgressPoint
{
    [SerializeField] private Employee _employee;

    public Employee Employee => _employee;

    public override void Activate()
    {
        base.Activate();
        _employee.gameObject.SetActive(true);
    }
}
