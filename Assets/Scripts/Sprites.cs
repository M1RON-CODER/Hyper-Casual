using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprites : MonoBehaviour
{
    [SerializeField] private GameObject _firTree;

    public GameObject FirTree => _firTree;

    public GameObject GetSprite(Resource.Resources resource)
    {
        switch(resource)
        {
            case Resource.Resources.FirTree:
                return _firTree;
            default:
                return null;
        }
    }
}
