using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Cash))]
public class CashRegister : MonoBehaviour
{
    public class Queue
    {
        private Transform _position;
        private AIController _AI;
        private bool _isBusyPlace;
        private bool _onSpot;

        public Queue(Transform position)
        {
            _position = position;
            _AI = null;
            _isBusyPlace = false;
        }

        public Transform Position => _position;
        public AIController AI => _AI;
        public bool IsBusyPlace => _isBusyPlace;
        public bool OnSpot => _onSpot;

        public void SetBusyPlace(AIController AI)
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
    [SerializeField] private Transform _boxPosition;
    [SerializeField] private List<Transform> _AIPositions = new ();

    private List<Queue> _queues = new List<Queue>();
    private Cash _cash;
    private bool _isHaveCashier;
    private bool _isHavePlayer;

    public List<Queue> Queues => _queues;

    #region MonoBehaviour
    private void Awake()
    {
        Initialize();

        _cash = GetComponent<Cash>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            _isHavePlayer = true;
            
            _cash.WithdrawCash(player); 
            
            if(!_isHaveCashier)
                Serve();
        }
        
        if (other.TryGetComponent(out Cashier AI))
        {
            // _isHavePlayer = true;
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
            if ((queue.AI != null) && (queue.OnSpot) && (queue.AI.ResourcesInHands.Count > 0))
            {
                //Box box = InstantiateBox();
                InstantiateBox(queue);
            }
        }
    }
   
    public int UnoccupiedPlace()
    {
        return _queues.FindIndex(q => !q.IsBusyPlace);
    }

    public int CurrentIndex(AIController AI)
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
    private IEnumerator MoveResourcesToBox(Queue queue, Box box)
    {
        int index = 0;
        int cash = queue.AI.ResourcesInHands.Count * _cash.ProductCost;
        float duration = 0.4f;
        
        queue.SetOnSpot(false);

        // ОШИБКА
        // InvalidOperationException: Sequence contains no elements
        GameObject lastResource = queue.AI.ResourcesInHands.Last().gameObject;
            
        foreach (GameObject resource in queue.AI.ResourcesInHands.ToList())
        {
            if (index >= box.Positions.Count - 1)
            {
                index = 0;
            }

            queue.AI.ResourcesInHands.Remove(resource);
            resource.transform.SetParent(box.transform);

            if (resource.Equals(lastResource))
            {
                resource.transform.DOLocalJump(box.Positions[index].localPosition, 250, 1, duration).OnComplete(() =>
                {
                    MoveBoxToHands(box, queue.AI, queue.AI.MoveTowardsExit, cash);
                });
                yield break;
            }
            else
            {
                resource.transform.DOLocalJump(box.Positions[index].localPosition, 250, 1, duration);
            }

            index++;

            yield return new WaitForSeconds(duration);
        }
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

    private void MoveBoxToHands(Box box, AIController AI, Action moveTowardsExit, int cash)
    {
        float duration = 0.4f;
        AI.ResourcesInHands.Add(box.gameObject);
        AI.Move();

        box.transform.SetParent(AI.Hands.transform);
        box.transform.DOLocalRotate(new Vector3(0, 90, 0), duration);
        box.transform.DOLocalMove(Vector3.zero, duration).OnComplete(() => 
        {
            moveTowardsExit.Invoke();
            AI.AIManager.RemoveAIFromQueue(AI);
            _cash.AddCashOnCashRegister(AI.transform, cash);
            NextBuyer();
        });
    }

    private void InstantiateBox(Queue queue)
    {

        Box box = Instantiate(_boxPrefab, _boxPosition.position, Quaternion.identity);
        Vector3 endScale = box.transform.localScale;
        box.transform.localScale = Vector3.zero;
        box.transform.DOScale(endScale, 0.5f).OnComplete(() =>
        {
            StartCoroutine(MoveResourcesToBox(queue, box));
        });
    }
}