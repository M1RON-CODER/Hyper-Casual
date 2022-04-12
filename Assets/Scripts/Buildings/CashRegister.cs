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
            _bot = bot;
            cashRegister.ServeBot(this);
        }
    }

    [SerializeField] private Box _boxPrefab;
    [SerializeField] private Transform _boxPosition;
    [SerializeField] private List<BotParams> _botParams = new List<BotParams>();

    private bool _isHaveEmployee;

    public List<BotParams> PositionForBots => _botParams;

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

    public void ServeBot(BotParams botParams)
    {
        if (!CheckFirstBot(botParams))
        {
            return;
        }

        BotParams bot = _botParams.First();

        MoveResourcesInBox(bot.Bot);
    }

    public Transform GetPosition()
    {
        foreach (var position in _botParams)
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
        for (int i = 0; i < _botParams.Count; i++)
        {
            if(_botParams[i].position == position)
            {
                return i;
            }
        }
        
        return -1;
    }

    private void MoveResourcesInBox(BotController bot)
    {
        Box box = Instantiate(_boxPrefab.gameObject, _boxPosition.position, Quaternion.identity).GetComponent<Box>();

        int index = 0;
        float duration = 0.3f;
        foreach (GameObject resource in bot.ResourcesInHands.ToList())
        {
            if(index > box.Positions.Count - 1)
            {
                index = 0;
            }

            resource.transform.SetParent(box.transform);
            resource.transform.DOLocalJump(box.Positions[index].position, 250, 1, duration);

            index++;
        }

        bot.SetAnimation(Keys.CarryingIdle, false);
        bot.SetAnimation(Keys.Idle, true);   
        
        box.transform.SetParent(bot.Hands.transform);
        box.transform.DOLocalMove(Vector3.zero, 0.3f);
        Invoke(nameof(NextBuyer), bot.ResourcesInHands.Count * duration + 0.3f);
    }

    private Box InstantiateBox()
    {
        return Instantiate(_boxPrefab, _boxPosition.position, Quaternion.identity).GetComponent<Box>();
    }

    private void NextBuyer()
    {
        _botParams.First().Bot.MoveTowardsExit();

        if (GetBusyPlace())
        {
            for (int i = 0; i < _botParams.Count; i++)
            {
                if(i == _botParams.Count - 1)
                {
                    _botParams[i] = new BotParams();
                }
                
                _botParams[i] = _botParams[i + 1];
                _botParams[i].Bot.MovePosition(_botParams[i].position);
            }

            ServeBot(_botParams.First());
        }
    }

    private bool GetBusyPlace()
    {
        foreach(BotParams bot in _botParams.ToList())
        {
            if (bot.IsBusyPlace)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckFirstBot(BotParams bot)
    {
        return _botParams.FindIndex(x => x == bot) == 0;
    }
}
