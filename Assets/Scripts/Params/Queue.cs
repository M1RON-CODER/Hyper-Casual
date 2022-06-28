using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queue : MonoBehaviour
{
    private Transform _position;
    private Buyer _AI;
    private bool _isBusyPlace;
    private bool _onSpot;

    public Queue(Transform position)
    {
        _position = position;
        _AI = null;
        _isBusyPlace = false;
    }

    public Transform Position => _position;
    public Buyer AI => _AI;
    public bool IsBusyPlace => _isBusyPlace;
    public bool OnSpot => _onSpot;

    public void SetBusyPlace(Buyer AI)
    {
        _AI = AI;
        _isBusyPlace = true;
    }

    public void SetOnSpot(bool onSpot)
    {
        _onSpot = onSpot;
    }
}
