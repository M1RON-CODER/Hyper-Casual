using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class Player : MonoBehaviour, IHumanoid
{
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private Transform _handsPosition;
    [SerializeField] private CashData _cashData;

    private List<Resource.ResourceBase> _resourcesOnHands = new ();
    private Animator _animator;
    private Rigidbody _rigidbody;
    private int _maxCountOnHands = 3;

    public CashData CashData => _cashData;
    public GameObject PlayerObject => _playerObject;
    public Transform Hands => _handsPosition;
    public List<Resource.ResourceBase> ResourcesOnHands => _resourcesOnHands;
    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;
    public int MaxCountOnHands => _maxCountOnHands;

    #region MonoBehaviour
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        
        _maxCountOnHands = PlayerPrefs.GetInt(Keys.PlayerPrefs.PlayerMaxCountOnHands.ToString(), 3);
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

    private void IncreaseMaxCountOnHands()
    {
        _maxCountOnHands++;
        PlayerPrefs.SetInt(Keys.PlayerPrefs.PlayerMaxCountOnHands.ToString(), MaxCountOnHands);
    }
}