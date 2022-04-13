using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class BotController : MonoBehaviour
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

    [SerializeField] private GameObject _hands;
    [SerializeField] private BotBar _botBar;

    private List<TargetParams> _targets = new List<TargetParams>();
    private List<GameObject> _resourcesInHands = new List<GameObject>();

    private NavMeshAgent _agent;
    private Animator _animator;
    private CashRegister _cashRegister;
    private BotManager _botManager;
    private Transform _exitPoint; 
    private string _currentAnimation = Keys.Idle;

    public GameObject Hands => _hands;
    public List<GameObject> ResourcesInHands => _resourcesInHands;
    public NavMeshAgent Agent => _agent;
    public BotManager Manager => _botManager;

    #region MonoBehaviour
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    #endregion

    public void Initialize(BotManager botManager, CashRegister cashRegister, Transform exitPoint)
    {
        _botManager = botManager;
        _cashRegister = cashRegister;
        _exitPoint = exitPoint;
    }

    public void Move()
    {
        _agent.isStopped = false;

        SettingAnimation();
        _animator.SetBool(_currentAnimation, true);
    }

    public void Stop()
    {
        _agent.isStopped = true;

        SettingAnimation();
        _animator.SetBool(_currentAnimation, false);
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
        TargetParams target = _targets.FirstOrDefault();
        if ((target == null) || (target.CurrentCountResources >= target.TotalCountResources))
        {
            return true;
        }

        float resourcePositionY = _resourcesInHands.Count == 0 ? 0 : _resourcesInHands.Count * resource.transform.localScale.y;
        resource.transform.SetParent(_hands.transform);
        resource.transform.DOLocalMove(new Vector3(0, resourcePositionY, 0), 0.2f);
        
        _resourcesInHands.Add(resource);

        target.CurrentResourcePlusOne();

        _botBar.UpdateData(target.CurrentCountResources, target.TotalCountResources);

        Stop();

        return target.CurrentCountResources >= target.TotalCountResources;
    }

    public void RemoveResourceInHands(GameObject resource)
    {
        resource.transform.SetParent(null);
        _resourcesInHands.Remove(resource);
    }

    public void MovePosition(Transform point)
    {        
        _agent.destination = point.position;
        Move();
        StartCoroutine(CheckDistanceStop());
    }
   
    // Ошибка 
    public void NextTarget()
    {
        // При удаление первого элемента в списке происходит ошибка
        _targets.Remove(_targets.First());
        if(_targets.Count == 0)
        {
            MoveToCashRegister();
        }
        else
        {
            MoveWaypoint();
        }
    }

    public void MoveTowardsExit()
    {
        Move();
        _agent.destination = _exitPoint.position;
        StartCoroutine(CheckDistanceForDestroy());
    }

    private void MoveWaypoint()
    {
        TargetParams target = _targets.First();
        _agent.destination = target.Waypoint.position;
        
        Move();

        _botBar.SetData(target.Resource, target.CurrentCountResources, target.TotalCountResources);
    }

    private void MoveToCashRegister()
    {
        // Debug.Log($"{this.name} MoveToCashRegister");
        int index = _cashRegister.UnoccupiedPlace();
        CashRegister.Queue queue = _cashRegister.Queues[index];
        queue.SetBusyPlace(this);

        _agent.destination = queue.Position.position;

        StartCoroutine(CheckDistanceStop());

        Move();
    }

    private IEnumerator CheckDistanceStop()
    {
        // Debug.Log($"{this.name} Check Distance Stop");
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            
            if (_agent.remainingDistance <= 0.1f)
            {
                // Debug.Log($"{this.name} is stoped");
                Stop();

                int index = _cashRegister.CurrentIndex(this);

                if(index == 0)
                {
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
            if (_agent.remainingDistance <= 1f)
            {
                _botManager.DestroyBot(this);
                yield break;
            }
        }
    }

    private void SettingAnimation()
    {
        if (_resourcesInHands.Count > 0)
        {
            _animator.SetBool(Keys.CarryingIdle, true);
            _animator.SetBool(Keys.CarryingWalking, true);

            _animator.SetBool(Keys.Idle, false);
            _animator.SetBool(Keys.Walking, false);

            _currentAnimation = Keys.CarryingWalking;
        }
        else
        {
            _animator.SetBool(Keys.Idle, true);
            _animator.SetBool(Keys.Walking, true);

            _animator.SetBool(Keys.CarryingIdle, false);
            _animator.SetBool(Keys.CarryingWalking, false);

            _currentAnimation = Keys.Walking;
        }
    }

}
