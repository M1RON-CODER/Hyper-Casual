using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Buyer : AI
{
    public class TargetParams
    {
        private Transform _rack;
        private Resource.Resources _resource;
        private int _coutResources;
        private int _totalCountResources;

        public TargetParams(Transform rack, Resource.Resources resource, int totalCountResources)
        {
            _rack = rack;
            _resource = resource;
            _totalCountResources = totalCountResources;
        }

        public Transform Rack => _rack;
        public Resource.Resources Resource => _resource;
        public int CountResources => _coutResources;
        public int TotalCountResources => _totalCountResources;

        public void CurrentResourcePlusOne()
        {
            _coutResources++;
        }
    }

    private List<TargetParams> _targets = new();
    private CashRegister _cashRegister;
    private AIManager _AIManager;
    private TargetParams _currentTarget;
    private Transform _exitPoint;
    private Sequence _sequence = DOTween.Sequence();
    private string _currentAnimation = Keys.Animation.Idle.ToString();

    public AIManager AIManager => _AIManager;
    public TargetParams CurrentTarget => _currentTarget;

    public void Initialize(AIManager AIManager, CashRegister cashRegister, Transform exitPoint)
    {
        _AIManager = AIManager ?? throw new System.ArgumentNullException(nameof(AIManager));
        _cashRegister = cashRegister ?? throw new System.ArgumentNullException(nameof(cashRegister));
        _exitPoint = exitPoint ?? throw new System.ArgumentNullException(nameof(exitPoint));
    }

    public override void Move()
    {
        base.Move();

        AnimationAdjustment();
        Animator.SetBool(_currentAnimation, true);
    }

    public override void Stop()
    {
        base.Stop();

        AnimationAdjustment();
        Animator.SetBool(_currentAnimation, false);
    }

    public void SetTargets(List<Rack> racks)
    {
        foreach (Rack rack in racks)
        {
            _targets.Add(new TargetParams(rack.BotPosition, rack.CurrentResource, Random.Range(1, 4)));
        }

        MoveWaypoint();
    }

    public bool AddResourceOnHands(GameObject resource)
    {
        if(_currentTarget == null)
        {
            return false;
        }
        
        Vector3 position = GetPositionForResourceOnHands();
        resource.transform.SetParent(Hands);
        _sequence.Append(resource.transform.DOLocalJump(position, 1, 1, 0.3f).OnStart(() => 
        {
            _currentTarget.CurrentResourcePlusOne();
            AIBar.RefreshData(_currentTarget.CountResources, _currentTarget.TotalCountResources);

        }).OnComplete(() =>
        {
            if (_currentTarget.CountResources == _currentTarget.TotalCountResources)
            {
                _sequence.OnComplete(() =>
                {
                    NextTarget();
                });
            }
        }));
            
        ResourcesOnHands.Insert(0, resource);
        
        AnimationAdjustment();
        Animator.SetBool(_currentAnimation, false);

        return _currentTarget.CountResources >= _currentTarget.TotalCountResources;
    }

    public void AddBoxToHands(GameObject box)
    {
        Vector3 position = GetPositionForResourceOnHands();
        box.transform.SetParent(Hands);
        box.transform.DOLocalMove(position, 0.2f);
        ResourcesOnHands.Add(box);

        AnimationAdjustment();
    }

    public void RemoveResourceFromHands(GameObject resource)
    {
        resource.transform.SetParent(null);
        ResourcesOnHands.Remove(resource);

        AnimationAdjustment();
        Animator.SetBool(_currentAnimation, false);
    }
    
    public void NextTarget()
    {
        if (_targets.Count == 0) 
        {
            throw new System.Exception("No targets");
        }

        if(_targets.Count - 1 > 0)
        {
            _targets.Remove(_targets.FirstOrDefault());
            
            _currentTarget = _targets.First();
            MoveWaypoint();
        }
        else
        {
            _currentTarget = null;
            
            MoveToCashRegister();
        }

    }

    public void MovePosition(Transform point)
    {        
        Agent.destination = point.position;
        Move();
        StartCoroutine(StopDistance());
    }
   

    public void MoveTowardsExit()
    {
        MovePosition(_exitPoint);
        AIBar.Hide();

        StartCoroutine(CheckDistanceForDestroy());
    }

    private void MoveWaypoint()
    {
        TargetParams target = _targets.First();
        _currentTarget = target;
        
        Agent.destination = target.Rack.position;
        Move();
        
        AIBar.SetData(target.Resource, target.CountResources, target.TotalCountResources);
    }

    private void MoveToCashRegister()
    {
        int index = _cashRegister.UnoccupiedPlace();
        CashRegister.Queue queue = _cashRegister.Queues[index];
        queue.SetBusyPlace(this);
        
        MovePosition(queue.Position);
        StartCoroutine(StopDistanceCashRegister());
        AIBar.SetData(Building.Buildings.CashRegister);
    }

    private Vector3 GetPositionForResourceOnHands()
    {
        if (ResourcesOnHands.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 position = Vector3.zero;
        foreach (GameObject resource in ResourcesOnHands)
        {
            position.y += resource.transform.localScale.y;
        }
        
        return position;
    }

    private IEnumerator StopDistance()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            
            if (Agent.remainingDistance <= 0.1f)
            {
                Stop();
                yield break;
            }
        }
    }

    private IEnumerator StopDistanceCashRegister()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            if (Agent.remainingDistance <= 0.1f)
            {
                int index = _cashRegister.CurrentIndex(this);
                if (index == 0)
                {
                    if (_cashRegister.Queues[index].OnSpot)
                    {
                        yield return null;
                    }

                    _cashRegister.Queues[index].SetOnSpot(true);
                    _cashRegister.Serve();

                    yield break;
                }
            }
        }
    }

    private IEnumerator CheckDistanceForDestroy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (Agent.remainingDistance <= 1f)
            {
                _AIManager.DestroyAI(this);
                yield break;
            }
        }
    }

    private void AnimationAdjustment()
    {
        if (ResourcesOnHands.Count > 0)
        {
            Animator.SetBool(Keys.Animation.CarryingIdle.ToString(), true);
            Animator.SetBool(Keys.Animation.CarryingWalking.ToString(), true);

            Animator.SetBool(Keys.Animation.Idle.ToString(), false);
            Animator.SetBool(Keys.Animation.Walking.ToString(), false);

            _currentAnimation = Keys.Animation.CarryingWalking.ToString();
        }
        else
        {
            Animator.SetBool(Keys.Animation.Idle.ToString(), true);
            Animator.SetBool(Keys.Animation.Walking.ToString(), true);

            Animator.SetBool(Keys.Animation.CarryingIdle.ToString(), false);
            Animator.SetBool(Keys.Animation.CarryingWalking.ToString(), false);

            _currentAnimation = Keys.Animation.Walking.ToString();
        }
    }
}
