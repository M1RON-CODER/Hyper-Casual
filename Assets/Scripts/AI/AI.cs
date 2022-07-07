using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class AI : MonoBehaviour, IHumanoid
{
    [SerializeField] private Renderer _skin;
    [SerializeField] private Transform _handsPosition;
    [SerializeField] private AIBar _AIBar;

    private List<GameObject> _resourcesOnHands = new ();
    private NavMeshAgent _agent;
    private Animator _animator;

    public Renderer Skin => _skin;
    public Transform Hands => _handsPosition;
    public AIBar AIBar => _AIBar;
    public List<GameObject> ResourcesOnHands => _resourcesOnHands;
    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;

    #region MonoBehaviour
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    #endregion

    public virtual bool AddResourceOnHands(Resource.Resources resource, GameObject resourceObj)
    {
        return false;
    }

    public virtual void RemoveResourceFromHands(Resource.ResourceBase resource)
    {
    }

    public virtual void AnimationAdjustment()
    {
    }

    public virtual void Move()
    {
        _agent.isStopped = false;
    }

    public virtual void Stop()
    {
        _agent.isStopped = true;
    }

    private void FacingTarget()
    {
    }
}
