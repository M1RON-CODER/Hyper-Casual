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
        public int CurrrentCountResources { get { return _currentCoutResources; } set { _currentCoutResources = value; } }
    }

    public bool IsStopped { get; set; }

    [SerializeField] private GameObject _hands;
    
    // ????
    private List<Resource> _waypoints = new List<Resource>();
    //

    private List<GameObject> _cashRegisters = new List<GameObject>();
    private List<TargetParams> _targets = new List<TargetParams>();
    private List<GameObject> _resourcesOnHands = new List<GameObject>();
    private NavMeshAgent _agent;
    private Animator _animator;
    private Sprites _sprites;
    private string _currentAnimation = Keys.Idle;

    #region MonoBehaviour
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        
        SetAnimation(Keys.Idle, true);
    }
    #endregion

    public void Initialize(List<Resource> waypoints, List<GameObject> cashRegisters, Sprites sprites)
    {
        _waypoints = waypoints;
        _cashRegisters = cashRegisters;
        _sprites = sprites;
    }

    // ПОФИКСИТЬ!!!!-------- БАГ: Постройка не имеет координать Ошибка gameObject строка 71 
    public void SetWaypoints()
    {
        List<Resource> buildings = new List<Resource>(_waypoints);
        for (int i = 0; i < (_waypoints.Count >= 4 ? Random.Range(1, 4) : Random.Range(1, _waypoints.Count)); i++)
        {
            int index = Random.Range(0, _waypoints.Count);
            
            _targets.Add(new TargetParams(buildings[index].gameObject.transform.position, buildings[index].CurrentResource, Random.Range(1, 4)));
            buildings.RemoveAt(index);
        }

        MoveWaypoint();
    }
    // ----------------------------
    public void OnObjectEnter(ResourceStorage resourceStorage)
    {
        Stop();
        AddResourceOnHands(resourceStorage);
        SetAnimation(Keys.Walking, false);
    }

    public void OnObjectExit()
    {
        Move();
        SetAnimation(Keys.Walking, true);
    }

    public void AddResourceOnHands(ResourceStorage resourceStorage)
    {
        for (int i = 0; i < _targets.First().TotalCountResources; i++) 
        {
            _resourcesOnHands.Add(resourceStorage.RemoveResource(_hands));
        }
    }

    public void RemoveOnHands(GameObject resource)
    {
        _resourcesOnHands.Remove(resource);
    }

    private void MoveWaypoint()
    {
        _agent.destination = _targets.First().Waypoint;
        
        // AddResourceOverhead();
    }
    
    private void AddResourceOverhead()
    {
        GameObject resourceSprite = Instantiate(_sprites.GetSprite(_targets.First().Resource), transform.GetChild(0)) as GameObject;
        resourceSprite.transform.localPosition = new Vector3(0, 2.5f, 0);
        resourceSprite.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    
    private void SetAnimation(string key, bool value)
    {
        _animator.SetBool(key, value);
    }


    private void Move() => _agent.isStopped = false;
    private void Stop() => _agent.isStopped = true;
}
