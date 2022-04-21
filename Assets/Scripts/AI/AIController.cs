using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : AI
{
    private class TargetParams
    {
        private Transform _waypoint;
        private Resource.Resources _resource;
        private int _currentCoutResources;
        private int _totalCountResources;

        public TargetParams(Transform wayPoint, Resource.Resources resource, int totalCountResources)
        {
            _waypoint = wayPoint;
            _resource = resource;
            _totalCountResources = totalCountResources;
        }

        public Transform Waypoint => _waypoint;
        public Resource.Resources Resource => _resource;
        public int CurrentCountResources => _currentCoutResources;
        public int TotalCountResources => _totalCountResources;

        public void CurrentResourcePlusOne()
        {
            _currentCoutResources++;
        }
    }

    private List<TargetParams> _targets = new();
    private CashRegister _cashRegister;
    private AIManager _AIManager;
    private Transform _exitPoint;
    private string _currentAnimation = Keys.Idle;

    public AIManager AIManager => _AIManager;

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

    public void SetTargets(List<GameObject> waypoints)
    {
        foreach (GameObject waypoint in waypoints)
        {
            _targets.Add(new TargetParams(waypoint.transform, waypoint.GetComponent<Resource>().CurrentResource, Random.Range(1, 4)));
        }

        MoveWaypoint();
    }

    public bool AddResourceToHands(GameObject resource)
    {
        AnimationAdjustment();
        
        TargetParams target = _targets.FirstOrDefault();
        if ((target == null) || (target.CurrentCountResources >= target.TotalCountResources))
        {
            NextTarget();
            return true;
        }

        Vector3 position = GetPositionForResourceOnHands();
        resource.transform.DOLocalMove(position, 0.2f);
        ResourcesOnHands.Insert(0, resource);

        AIBar.RefreshData(target.CurrentCountResources, target.TotalCountResources);

        target.CurrentResourcePlusOne();

        return target.CurrentCountResources >= target.TotalCountResources;
    }

    public void RemoveResourceFromHands(GameObject resource)
    {
        resource.transform.SetParent(null);
        ResourcesOnHands.Remove(resource);

        AnimationAdjustment();
    }
    
    public void NextTarget()
    {
        if (_targets.Count == 0) 
        {
            throw new System.Exception("No targets");
        }

        _targets.Remove(_targets.FirstOrDefault());
        if(_targets.Count == 0)
        {
            MoveToCashRegister();
        }
        else
        {
            MoveWaypoint();
        }
    }

    public void MovePosition(Transform point)
    {        
        Agent.destination = point.position;
        Move();
        StartCoroutine(StopDistanceCheck());
    }
   

    public void MoveTowardsExit()
    {
        MovePosition(_exitPoint);
        AIBar.Hide();
    }

    private void MoveWaypoint()
    {
        TargetParams target = _targets.First();
        Agent.destination = target.Waypoint.position;
        Move();
        
        AIBar.SetData(target.Resource, target.CurrentCountResources, target.TotalCountResources);
    }

    private void MoveToCashRegister()
    {
        int index = _cashRegister.UnoccupiedPlace();
        CashRegister.Queue queue = _cashRegister.Queues[index];
        queue.SetBusyPlace(this);
        
        MovePosition(queue.Position);
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
            position.y += resource.transform.localScale.x;
        }

        return position;
    }

    private IEnumerator StopDistanceCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            
            if (Agent.remainingDistance <= 0.1f)
            {
                Stop();

                int index = _cashRegister.CurrentIndex(this);
                if(index == 0)
                {
                    _cashRegister.Queues[index].SetOnSpot(true);
                    _cashRegister.Serve();
                }

                yield break;
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
            Animator.SetBool(Keys.CarryingIdle, true);
            Animator.SetBool(Keys.CarryingWalking, true);

            Animator.SetBool(Keys.Idle, false);
            Animator.SetBool(Keys.Walking, false);

            _currentAnimation = Keys.CarryingWalking;
        }
        else
        {
            Animator.SetBool(Keys.Idle, true);
            Animator.SetBool(Keys.Walking, true);

            Animator.SetBool(Keys.CarryingIdle, false);
            Animator.SetBool(Keys.CarryingWalking, false);

            _currentAnimation = Keys.Walking;
        }
    }
}
