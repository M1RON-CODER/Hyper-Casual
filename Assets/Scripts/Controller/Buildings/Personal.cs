using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personal : MonoBehaviour
{
    private enum Method
    {
        LevelUp,
    }

    [SerializeField] private Method _method;

    private Dictionary<Method, System.Action> _methodLookup;
    private Collider _collider;

    private void Awake()
    {
        _methodLookup = new Dictionary<Method, System.Action>()
        {
            {Method.LevelUp, LevelUp },
        };
    }

    private void LevelUp()
    {
        if (_collider.GetComponent<PlayerController>())
        {
            _collider.GetComponent<PlayerController>().IncreaseMaxCountOnHands();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _collider = other;
        _methodLookup[_method].Invoke();
    }
}
