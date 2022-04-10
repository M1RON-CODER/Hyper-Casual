using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
    [Serializable]
    public class PositionParams
    {
        public Transform position;
        private bool _isFreeSpace;
        private bool _inPlace;

        public bool IsFreeSpace => _isFreeSpace;

        public void SetFreeSpace(bool isFreeSpace)
        {
            _isFreeSpace = isFreeSpace;
        }

        public void ComeToWaypoint(bool inPlace)
        {
            _inPlace = inPlace;
        }
    }

    [SerializeField] private List<PositionParams> _positionsForBots = new List<PositionParams>();

    private bool _isHaveEmployee;

    public List<PositionParams> PositionForBots => _positionsForBots;

    #region MonoBehaviour
    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            _isHaveEmployee = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _isHaveEmployee = false;
        }
    }
    #endregion

    public Transform GetPosition()
    {
        foreach (var position in _positionsForBots)
        {
            if (!position.IsFreeSpace)
            {
                position.SetFreeSpace(true);
                return position.position;
            }
        }

        return null;
    }

    public int GetIndexBotPosition(Transform position)
    {
        for (int i = 0; i < _positionsForBots.Count; i++)
        {
            if(_positionsForBots[i].position == position)
            {
                return i;
            }
        }
        
        return -1;
    }
}
