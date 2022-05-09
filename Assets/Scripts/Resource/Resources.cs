using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public class ResourceBase
    {
        public GameObject Obj { get; set; }
        public Resources Resource { get; set; }
    }
    
    public enum Resources { FirTree, Robot, Shark, Amogus, Dog }
    [SerializeField] private Resources _resource;

    public Resources CurrentResource => _resource;
}
