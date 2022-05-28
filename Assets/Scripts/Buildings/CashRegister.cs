using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class CashRegister : MonoBehaviour
{
    public class Queue
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
    
    [SerializeField] private Box _boxPrefab;
    [SerializeField][JsonProperty("Cash")] private Cash _cash;
    [SerializeField] private Transform _boxPosition;
    [SerializeField] private List<Transform> _AIPositions = new();

    private List<Queue> _queues = new();
    private bool _isHaveCashier;
    private bool _isHavePlayer;
    private const string _fileName = "CashRegister.json";    

    public List<Queue> Queues => _queues;
    public Cash Cash => _cash;

    #region MonoBehaviour
    private void Awake()
    {
        Initialize();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            _isHavePlayer = true;
            
            if(!_isHaveCashier)
                Serve();
        }
        
        if (other.TryGetComponent(out Cashier AI))
        {
            _isHaveCashier = true;
            
            Serve();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isHaveCashier)
        {
            _isHavePlayer = false;
        }
    }
    #endregion

    public void Serve()
    {
        if (_isHavePlayer || _isHaveCashier)
        {
            Queue queue = _queues.FirstOrDefault();
            Sequence sequence = DOTween.Sequence();
            if ((queue.AI != null) && queue.OnSpot)
            {
                Box box = InstantiateBox(queue);
                sequence
                    .OnComplete(() => 
                    { 
                        MoveResourcesToBox(queue, box); 
                    })
                    .SetDelay(0.5f);
            }
        }
    }
   
    public int UnoccupiedPlace()
    {
        return _queues.FindIndex(q => !q.IsBusyPlace);
    }

    public int CurrentIndex(Buyer AI)
    {
        return _queues.FindIndex(q => q.AI == AI);
    }
    
    private void Initialize()
    {
        for (int i = 0; i < _AIPositions.Count; i++)
        {
            _queues.Add(new Queue(_AIPositions[i]));
        }
    }

    private void MoveResourcesToBox(Queue queue, Box box)
    {
        Sequence sequence = DOTween.Sequence();
        int indexPosition = 0;
        int cash = queue.AI.ResourcesOnHands.Count * _cash.ProductCost;
        float duration = 0.4f;

        foreach (GameObject resource in queue.AI.ResourcesOnHands.ToList())
        {
            if (indexPosition >= box.Positions.Count - 1)
            {
                indexPosition = 0;
            }

            queue.AI.RemoveResourceFromHands(resource);
            resource.transform.SetParent(box.transform);
            sequence.Append(resource.transform.DOLocalJump(box.Positions[indexPosition].localPosition, 250, 1, duration));

            indexPosition++;
        }

        sequence.OnComplete(() =>
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(MoveBoxToHands(box, queue.AI));
            sequence.OnComplete(() =>
            {
                queue.AI.AIManager.RemoveAIFromQueue(queue.AI);
                _cash.AddCashOnCashRegister(queue.AI.transform, cash);
                queue.AI.MoveTowardsExit();
                NextBuyer();
            });
        });
    }

    private void NextBuyer()
    {   
        for (int i = 0; i < _queues.Count - 1; i++)
        {
            if(_queues[i + 1].AI == null)
            {
                _queues[i] = new Queue(_AIPositions[i]);
                return;
            }
            
            _queues[i].SetBusyPlace(_queues[i + 1].AI);
            _queues[i].AI.MovePosition(_AIPositions[i]);
        }
    }

    private Tween MoveBoxToHands(Box box, Buyer AI)
    {
        float duration = 0.4f;
        
        AI.AddBoxToHands(box.gameObject);
        box.transform.SetParent(AI.Hands);
        box.transform.DOLocalRotate(new Vector3(0, 90, 0), duration);

        return box.transform.DOLocalMove(Vector3.zero, duration);
    }

    private Box InstantiateBox(Queue queue)
    {

        Box box = Instantiate(_boxPrefab, _boxPosition.position, Quaternion.identity);
        Vector3 endScale = box.transform.localScale;
        box.transform.localScale = Vector3.zero;
        box.transform.DOScale(endScale, 0.5f);

        return box;
    }
}