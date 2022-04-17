using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Progress : MonoBehaviour
{
    //[SerializeField] private EmployeeManager _employeeManager;
    [SerializeField] private BuildingsManager _buildingsManager;

    private List<List<MyClass>> _progress;
    private int _progressIndex;
    private int _underProgressIndex;

    #region MonoBehaviour
    private void Awake()
    {
        _progress = new List<List<MyClass>>()
        {
            new List<MyClass>()
            {
                //new MyClass(){ Obj = _employeeManager.Cashier.gameObject, Cost = 100 },
            },
        };

        _progressIndex = PlayerPrefs.GetInt(Keys.ProgressIndex, 0);
        _underProgressIndex = PlayerPrefs.GetInt(Keys.UnderProgressIndex, 0);
    }
    #endregion
}

class MyClass
{
    public GameObject Obj { get; set; }
    public Sprites Sprites { get; set; }
    public int Cost { get; set; }
}

