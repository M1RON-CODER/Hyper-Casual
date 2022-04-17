using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    [SerializeField] private GameObject _cashPrefab;
    [SerializeField] private Transform _cashPosition;
    [SerializeField] private List<Transform> _AIPositions = new List<Transform>();

    private List<Queue> _queues = new List<Queue>();
    private Transform _startPositionCash;
    private int _cashAmount;
    private int _productCost = 3;
    private bool _isHaveCashier;
    private bool _isHavePlayer;

    public List<Queue> Queues => _queues;

    #region MonoBehaviour
    private void Awake()
    {
        Initialize();
        
        _startPositionCash = _cashPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            _isHavePlayer = true;
            
            WithdrawCash(player); 
            Serve();
        }
        
        if (other.TryGetComponent(out Cashier AI))
        {
            _isHavePlayer = true;
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
        if (_isHavePlayer)
        {
            Queue queue = _queues.FirstOrDefault();
            if ((queue.AI != null) && (queue.OnSpot) && (queue.AI.ResourcesInHands.Count > 0))
            {
                Box box = InstantiateBox();
                StartCoroutine(MoveResourcesToBox(queue, box));
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
        int cash = queue.AI.ResourcesInHands.Count * _productCost;
        float duration = 0.4f;
        
        queue.SetOnSpot(false);
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
                    MoveBoxToHands(box, queue.AI, queue.AI.MoveTowardsExit);
                });

                AddCash(cash);
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

    private void MoveBoxToHands(Box box, AIController AI, Action moveTowardsExit)
    {
        float duration = 0.4f;

        AI.ResourcesInHands.Add(box.gameObject);

        box.transform.SetParent(AI.Hands.transform);
        box.transform.DOLocalRotate(new Vector3(0, 90, 0), duration);
        box.transform.DOLocalMove(Vector3.zero, duration).OnComplete(() => 
        {
            moveTowardsExit.Invoke();
            AI.AIManager.RemoveAIFromQueue(AI);
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


    private void AddCash(int cash)
    {
        var indexPosition = _cashAmount == 0 ? 0 : _cashAmount / _productCost;
        _cashAmount += cash;

        for (int i = 0; i < cash / _productCost; i++)
        {
            var positionY = 0;
            for (int j = 0; j < indexPosition * 9; j += 9)
            {
                if (j < j + 9)
                {
                    positionY = j;
                    break;
                }
            }
            
            if ((indexPosition + i) % 3 == 0)
            {
                _cashPosition.localPosition += new Vector3(0.55f, 0, -_startPositionCash.localPosition.z);
            }

            Instantiate(_cashPrefab, _cashPosition.position, Quaternion.identity);

            _cashPosition.localPosition += new Vector3(0, 0, -0.35f);
        }
    }

    private void WithdrawCash(Player player)
    {
        player.CashData.AddCash(_cashAmount);
        _cashAmount = 0;
    }
}