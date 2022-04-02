
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _buldings = new List<GameObject>();

    public bool IsStopped { get; set; }

    private List<Resource.ResourceType> _resourceOnHands = new List<Resource.ResourceType>();
    private NavMeshAgent _agent;
    private Animator _animator;
    private string _currentAnimation = Keys.Idle;

    #region MonoBehaviour
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();


        SetAnimation(Keys.Walking, true);
        _agent.destination = _buldings.First().transform.position;
    }

    #endregion
    public void OnObjectEnter()
    {
        SetAnimation(Keys.Walking, false);
    }

    public void OnObjectExit()
    {
        SetAnimation(Keys.Walking, true);
    }

    private void AddOnHands(Resource.ResourceType resource)
    {
        _resourceOnHands.Add(resource);
    }

    private void RemoveOnHands(Resource.ResourceType resource)
    {
        _resourceOnHands.Remove(resource);
    }

    private void SetAnimation(string key, bool value)
    {
        _animator.SetBool(key, value);
    }
}
