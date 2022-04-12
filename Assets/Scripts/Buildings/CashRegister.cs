using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CashRegister : MonoBehaviour
{
    public class Queue
    {
        private Transform _position;
        private BotController _bot;
        private bool _isBusyPlace;

        public Queue(Transform position)
        {
            _position = position;
            _bot = null;
            _isBusyPlace = false;
        }

        public Transform Position => _position;
        public BotController Bot => _bot;
        public bool IsBusyPlace => _isBusyPlace;

        public void SetBusyPlace(BotController bot)
        {
            _bot = bot;
            _isBusyPlace = true;
        }
    }

    [SerializeField] private Box _boxPrefab;
    [SerializeField] private Transform _boxPosition;
    [SerializeField] private List<Transform> _positions = new List<Transform>();

    private List<Queue> _queues = new List<Queue>();
    private bool _isHaveEmployee;

    public List<Queue> Queues => _queues;

    #region MonoBehaviour
    private void Awake()
    {
        Initialize();

        Debug.Log(UnoccupiedPlace());
    }
    #endregion

    public void Serve()
    {
        Queue queue = _queues.First();

        Box box = InstantiateBox();
        MoveResourcesToBox(queue, box);
    }

    public int UnoccupiedPlace()
    {
        return _queues.FindIndex(q => !q.IsBusyPlace);
    }

    private void MoveResourcesToBox(Queue queue, Box box)
    {
        int index = 0;
        float duration = 0.3f;
        foreach (GameObject resource in queue.Bot.ResourcesInHands.ToList())
        {
            if (index >= queue.Bot.ResourcesInHands.Count - 1)
            {
                index = 0;
            }

            queue.Bot.ResourcesInHands.Remove(resource);
            resource.transform.SetParent(box.transform);
            resource.transform.DOLocalJump(box.Positions[index].position, 250, 1, duration);
            
            index++;
        }
        
        MoveBoxToHands(box, queue.Bot.Hands.transform, queue.Bot.MoveTowardsExit);
        NextBuyer();   
    }

    private void NextBuyer()
    {
        for (int i = 0; i < _queues.Count; i++)
        {
            if(i == _queues.Count - 1)
            {
                _queues[i] = new Queue(_queues[i].Position);
            }

            _queues[i].SetBusyPlace(_queues[i + 1].Bot);
        }

        if(_queues.First().Bot != null)
        {
            Serve();
        }
    }

    private void MoveBoxToHands(Box box, Transform hands, Action moveTowardsExit)
    {
        float duration = 0.3f;
        box.transform.DORotate(new Vector3(0, 90, 0), duration);
        box.transform.SetParent(hands);
        box.transform.DOMove(Vector3.zero, duration).OnComplete(moveTowardsExit.Invoke);
    }

    private Box InstantiateBox()
    {
        return Instantiate(_boxPrefab, _boxPosition.position, Quaternion.identity);
    }

    private void Initialize()
    {
        for (int i = 0; i < _positions.Count; i++)
        {
            _queues.Add(new Queue(_positions[i]));
        }
    }
}


/*    [Serializable]
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
    }*/
