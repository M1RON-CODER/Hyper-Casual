using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assistant : Employee
{
    [SerializeField] private Transform _productionResource;
    [SerializeField] private ResourceStorage _resourceStorage;

    public Transform ProductionResource => _productionResource;
    public Transform ResourceStorage => _resourceStorage.transform;

    #region MonoBehaviour
    private void Start()
    {
        Move(_productionResource);
    } 
    #endregion
}
