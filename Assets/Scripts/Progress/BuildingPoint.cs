using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingPoint : ProgressPoint
{
    [SerializeField] private Rack _rack;
    [SerializeField] private ProductionResource _productionResource;

    public Rack Rack => _rack;

    public override void Activate()
    {
        base.Activate();
        _rack.gameObject.SetActive(true);
        _productionResource.gameObject.SetActive(true);
    }
}
