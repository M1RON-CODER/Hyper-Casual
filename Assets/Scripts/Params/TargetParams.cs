using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetParams : MonoBehaviour
{
    private Transform _rack;
    private Resource.Resources _resource;
    private int _coutResources;
    private int _totalCountResources;

    public TargetParams(Transform rack, Resource.Resources resource, int totalCountResources)
    {
        _rack = rack;
        _resource = resource;
        _totalCountResources = totalCountResources;
    }

    public Transform Rack => _rack;
    public Resource.Resources Resource => _resource;
    public int CountResources => _coutResources;
    public int TotalCountResources => _totalCountResources;

    public void CurrentResourcePlusOne()
    {
        _coutResources++;
    }
}
