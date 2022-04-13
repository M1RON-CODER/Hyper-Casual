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
    }

    private void OnTriggerEnter(Collider other)
    {
        _isHaveEmployee = true;
        Serve();
    }

    private void OnTriggerExit(Collider other)
    {
        _isHaveEmployee = false;
    }
    #endregion

    public void Serve()
    {
        if (_isHaveEmployee)
        {
            Queue queue = _queues.FirstOrDefault();
            if ((queue.Bot != null) && (queue.Bot.ResourcesInHands.Count > 0))
            {
                Box box = InstantiateBox();
                MoveResourcesToBox(queue, box);
            }
        }
/*        Queue queue = _queues.First();

        Box box = InstantiateBox();
        MoveResourcesToBox(queue, box);*/
    }
   
    public int UnoccupiedPlace()
    {
        return _queues.FindIndex(q => !q.IsBusyPlace);
    }

    public int CurrentIndex(BotController bot)
    {
        return _queues.FindIndex(q => q.Bot == bot);
    }
    
    private void MoveResourcesToBox(Queue queue, Box box)
    {
        int index = 0;
        float duration = 0.7f;
        
        GameObject lastResource = queue.Bot.ResourcesInHands.Last().gameObject;
            
        foreach (GameObject resource in queue.Bot.ResourcesInHands.ToList())
        {
            if (index >= box.Positions.Count - 1)
            {
                index = 0;
            }

            queue.Bot.ResourcesInHands.Remove(resource);
            resource.transform.SetParent(box.transform);

            if (resource.Equals(lastResource))
            {
                resource.transform.DOLocalJump(box.Positions[index].localPosition, 250, 1, duration).OnComplete(() =>
                {
                    MoveBoxToHands(box, queue.Bot, queue.Bot.MoveTowardsExit);
                });
                return;
            }
            else
            {
                resource.transform.DOLocalJump(box.Positions[index].localPosition, 250, 1, duration);
            }

            index++;
        } 
    }

    private void NextBuyer()
    {
        for (int i = 0; i < _queues.Count - 1; i++)
        {
            if(_queues[i + 1].Bot == null)
            {
                _queues[i] = new Queue(_positions[i]);
                return;
            }
            
            _queues[i].SetBusyPlace(_queues[i + 1].Bot);
            _queues[i].Bot.MovePosition(_positions[i]);
        }
    }

    private void MoveBoxToHands(Box box, BotController bot, Action moveTowardsExit)
    {
        float duration = 0.4f;

        bot.ResourcesInHands.Add(box.gameObject);

        box.transform.SetParent(bot.Hands.transform);
        box.transform.DOLocalRotate(new Vector3(0, 90, 0), duration);
        box.transform.DOLocalMove(Vector3.zero, duration).OnComplete(() => 
        {
            moveTowardsExit.Invoke();
            bot.Manager.RemoveBotFromQueue(bot);
            NextBuyer();
        });
    }

    private Box InstantiateBox()
    {

        /*Box box = Instantiate(_boxPrefab, _boxPosition.position, Quaternion.identity);
        Vector3 endScale = box.transform.localScale;
        box.transform.localScale = Vector3.zero;
        box.transform.DOScale(endScale, 0.5f);*/

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