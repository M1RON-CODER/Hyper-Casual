using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public enum ResourceType { Bun, Tomato, Cucumber, Cabbage, Cheese, Beef, Bow, Chicken, Ketchup }
    [SerializeField] private ResourceType _resource;

    public ResourceType GetResource { get { return _resource; } }
}
