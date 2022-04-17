using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class Employee : MonoBehaviour
{  
    private NavMeshAgent _agent;
    private Animator _animator;
    
    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;

    #region MonoBehaviour
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    #endregion

    public void Move(Transform position)
    {
        _agent.destination = position.position;
        _animator.SetBool(Keys.Walking, true);
        StartCoroutine(CheckDistanseStop());
    }

    public void Stop()
    {
        _animator.SetBool(Keys.Walking, false);
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
}
