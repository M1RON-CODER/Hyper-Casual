using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
    [Serializable]
    public class BotParams
    {
        public Transform position;
        private BotController _bot;
        private bool _isBusyPlace;

        public BotController Bot => _bot;
        public bool IsBusyPlace => _isBusyPlace;

        public void SetBusyPlace(bool isFreeSpace)
        {
            _isBusyPlace = isFreeSpace;
        }

        public void ComeToWaypoint(CashRegister cashRegister, BotController bot)
        {
            cashRegister.ServeBot();
            _bot = bot;
        }
    }

    [SerializeField] private GameObject _boxPrefab;
    [SerializeField] private Transform _boxPosition;
    [SerializeField] private List<BotParams> _positionsForBots = new List<BotParams>();

    private bool _isHaveEmployee;

    public List<BotParams> PositionForBots => _positionsForBots;

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

    public void ServeBot()
    {
        return;
        BotParams bot = _positionsForBots.First();

        MoveResourcesInBox(bot.Bot.ResourcesInHands);
    }

    public Transform GetPosition()
    {
        foreach (var position in _positionsForBots)
        {
            if (!position.IsBusyPlace)
            {
                position.SetBusyPlace(true);
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

    private void MoveResourcesInBox(List<GameObject> resources)
    {
        Box box = InstantiateBox();

        int index = 0;
        foreach (GameObject resource in resources.ToList())
        {
            if(index > box.Positions.Count - 1)
            {
                index = 0;
            }

            resource.transform.SetParent(box.transform);
            resource.transform.DOLocalMove(box.GetComponent<Box>().Positions[index].position, 0.1f);

            index++;
        }

        Invoke(nameof(NextBuyer), resources.Count * 0.1f);
    }

    private Box InstantiateBox()
    {
        return Instantiate(_boxPrefab, _boxPosition.position, Quaternion.identity).GetComponent<Box>();
    }

    private void NextBuyer()
    {
        _positionsForBots.First().SetBusyPlace(false);

        if (GetBusyPlace())
        {
            for (int i = 0; i < _positionsForBots.Count; i++)
            {

            }
        }
    }

    private bool GetBusyPlace()
    {
        foreach(BotParams bot in _positionsForBots.ToList())
        {
            if (bot.IsBusyPlace)
            {
                return true;
            }
        }

        return false;
    }
}
