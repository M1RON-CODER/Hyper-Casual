using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class BotController : MonoBehaviour
{
    private class TargetParams
    {
        private Vector3 _waypoint;
        private Resource.Resources _resource;
        private int _totalCountResources;
        private int _currentCoutResources;

        public TargetParams(Vector3 wayPoint, Resource.Resources resource, int totalCountResources)
        {
            _waypoint = wayPoint;
            _resource = resource;
            _totalCountResources = totalCountResources;
        }

        public Vector3 Waypoint => _waypoint;
        public Resource.Resources Resource => _resource;
        public int TotalCountResources => _totalCountResources;
        public int CurrentCountResources => _currentCoutResources;

        public void AddOneResource()
        {
            _currentCoutResources++;
        }
    }

    public bool IsStopped { get; set; }

    [SerializeField] private GameObject _hands;
    [SerializeField] private BotBar _botBar;

    private List<TargetParams> _targets = new List<TargetParams>();
    private List<CashRegister> _cashRegisters = new List<CashRegister>();
    private List<GameObject> _resourcesInHands = new List<GameObject>();

    private NavMeshAgent _agent;
    private Animator _animator;
    private Sprites _sprites;
    private Transform _exit; 
    
    private string _currentAnimation = Keys.Idle;

    public GameObject Hands => _hands;
    public List<GameObject> ResourcesInHands => _resourcesInHands;
    public NavMeshAgent Agent => _agent;

    #region MonoBehaviour
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        SetAnimation(Keys.Idle, true);
    }
    #endregion

    public void Initialize(List<CashRegister> cashRegisters, Sprites sprites, Transform exit)
    {
        _cashRegisters = cashRegisters;
        _sprites = sprites;
        _exit = exit;
    }

    public void SetWaypoints(List<GameObject> waypoints)
    {
        List<GameObject> buildings = waypoints.ToList();
        for (int i = 0; i < (buildings.Count >= 4 ? Random.Range(1, 4) : Random.Range(1, buildings.Count)); i++)
        {
            int index = Random.Range(0, buildings.Count);
            
            _targets.Add(new TargetParams(buildings[index].gameObject.transform.position, buildings[index].GetComponent<Resource>().CurrentResource, Random.Range(1, 2)));
            buildings.RemoveAt(index);
        }

        MoveWaypoint();
    }
    public void OnObjectEnter(ResourceStorage resourceStorage)
    {
        StateStop();
        SetAnimation(Keys.Walking, false);
    }

    public void OnObjectExit()
    {
        StateMove();
        SetAnimation(_currentAnimation, true);
    }

    public bool AddResourceInHands(GameObject resource)
    {
        SetAnimation(Keys.CarryingIdle, true);

        float positionY = _resourcesInHands.Count == 0 ? 0 : _resourcesInHands.Count * resource.transform.localScale.y;

        resource.transform.SetParent(_hands.transform);
        resource.transform.DOLocalMove(new Vector3(0, positionY, 0), 0.2f);
        
        _resourcesInHands.Add(resource);

        _targets.First().AddOneResource();

        _botBar.UpdateData(_targets.First().CurrentCountResources, _targets.First().TotalCountResources);

        return _targets.First().CurrentCountResources >= _targets.First().TotalCountResources;
    }

    public void MovePosition(Transform point)
    {
        _agent.destination = point.position;
        StartCoroutine(CheckDistanceStop());
    }
   
    public IEnumerator NextTarget()
    {
        yield return new WaitForSeconds(0.2f);

        _currentAnimation = Keys.CarryingWalking;
        SetAnimation(Keys.CarryingWalking, true);

        if (_targets.Count > 0)
        {
            _targets.Remove(_targets.First());
            MoveCashRegister();
        }
        
        if (_targets.Count != 0)
        {
            MoveWaypoint();
        }
    }

    public void MoveTowardsExit()
    {
        _agent.destination = _exit.position;
    }

    public void SetAnimation(string key, bool value)
    {
        _animator.SetBool(key, value);
    }

    private void Move()
    {
        
    }

    private void Stop()
    {
        
    }

    private void MoveWaypoint()
    {
        TargetParams target = _targets.First();
        _agent.destination = target.Waypoint;

        _botBar.SetData(target.Resource, target.CurrentCountResources, target.TotalCountResources);

        // UpdateCountResourcesOverhead();
    }

    private void UpdateCountResourcesOverhead()
    {
        TargetParams target = _targets.First();
        // _demand.text = $"{target.CurrentCountResources} / {target.TotalCountResources}";
    }
    
    private void MoveCashRegister()
    {
        Dictionary<CashRegister, float> distance = new Dictionary<CashRegister, float>();
        
        foreach (CashRegister cashRegister in _cashRegisters)
        {
            distance.Add(cashRegister, Vector3.Distance(cashRegister.transform.position, transform.position));
        }

        var cashReg = distance.Where(x => x.Value == distance.Values.Min()).FirstOrDefault().Key;

        Transform botPosition = cashReg.GetPosition();
        Debug.Log("Bot position: " + botPosition);
        _agent.destination = botPosition.position;
        int indexPosition = cashReg.GetIndexBotPosition(botPosition);
        
        StartCoroutine(CheckDistanceStop(cashReg, indexPosition));

        StateMove();
    }

    private IEnumerator CheckDistanceStop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            if (_agent.remainingDistance <= 0.1f)
            {
                StateStop();

                yield break;
            }
        }
    }
    
    private IEnumerator CheckDistanceStop(CashRegister cashRegister, int indexPosition)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            if (_agent.remainingDistance <= 0.1f)
            {
                cashRegister.PositionForBots[indexPosition].ComeToWaypoint(cashRegister, this);
                StateStop();

                yield break;
            }
        }
    }

    private void StateMove()
    {
        _agent.isStopped = false;

        _animator.SetBool(_currentAnimation, true);
    }
    private void StateStop()
    {
        _agent.isStopped = true;

        _animator.SetBool(_currentAnimation, false);        
    }
}
