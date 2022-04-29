using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class Employee : MonoBehaviour
{
    [SerializeField] private Transform _hands;

    private Stack<Resource.ResourceParams> _resourcesOnHands = new();
    private NavMeshAgent _agent;
    private Animator _animator;
    private int _maxCountOnHands;
    private string _currentAnimation;
    
    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;

    #region MonoBehaviour
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _maxCountOnHands = PlayerPrefs.GetInt(Keys.AIPrefs.HelperMaxCountOnHands.ToString(), 3);
    }
    #endregion

    public bool AddResourceOnHands(Resource.Resources resource, GameObject resourceObj)
    {
        if (_resourcesOnHands.Count >= _maxCountOnHands)
        {
            return true;
        }

        Vector3 position = GetPositionForResourceOnHands();
        resourceObj.transform.SetParent(_hands);
        resourceObj.transform.DOLocalMove(position, 0.2f);

        _resourcesOnHands.Push(new Resource.ResourceParams { Obj = resourceObj, Resource = resource });

        AnimationAdjustment();

        return false;
    }

    public void Move(Transform position)
    {
        _agent.destination = position.position;
        _animator.SetBool(Keys.Animation.Walking.ToString(), true);
        StartCoroutine(CheckDistanseStop());
    }

    public void Stop()
    {
        _animator.SetBool(Keys.Animation.Walking.ToString(), false);
    }

    private IEnumerator CheckDistanseStop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            
            if (_agent.remainingDistance <= 0.1f)
            {
                Stop();
                yield break;
            }
        }
    }

    private Vector3 GetPositionForResourceOnHands()
    {
        if (_resourcesOnHands.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 position = Vector3.zero;
        foreach (Resource.ResourceParams resource in _resourcesOnHands)
        {
            position.y += resource.Obj.transform.localScale.y;
        }

        return position;
    }

    private void AnimationAdjustment()
    {
        if (_resourcesOnHands.Count > 0)
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
