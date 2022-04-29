using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : Employee
{
    [SerializeField] private Garden _garden;
    [SerializeField] private ResourceStorage _resourceStorage;

    #region MonoBehaviour
    private void Start()
    {
        Move(_garden.transform);
    } 
    #endregion
   
}
