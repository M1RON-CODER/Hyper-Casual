using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public enum ResourceType { FirTree }
    [SerializeField] private ResourceType _resource;

    public ResourceType GetResourceType { get { return _resource; } }
}
