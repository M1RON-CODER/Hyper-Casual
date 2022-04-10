using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
    [Serializable]
    private class PositionParams
    {
        public Transform position;
        private bool _isFreeSpace;

        public bool IsFreeSpace => _isFreeSpace;

        public void SetFreeSpace(bool isFreeSpace)
        {
            _isFreeSpace = isFreeSpace;
        }
    }

    [SerializeField] private List<PositionParams> _positionsForBots = new List<PositionParams>();

    #region MonoBehaviour
    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

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
}
