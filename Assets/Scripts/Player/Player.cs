using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public abstract class Player : MonoBehaviour
{
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private Transform _handsPosition;
    [SerializeField] private CashData _cashData;

    private List<Resource.ResourceParams> _resourcesOnHands = new();
    private Animator _animator;
    private Rigidbody _rigidbody;
    private int _maxCountOnHands = 3;

    public CashData CashData => _cashData;
    public GameObject PlayerObject => _playerObject;
    public Transform Hands => _handsPosition;
    public List<Resource.ResourceParams> ResourcesOnHands => _resourcesOnHands;
    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;
    public int MaxCountOnHands => _maxCountOnHands; 

    #region MonoBehaviour
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        
        _maxCountOnHands = PlayerPrefs.GetInt(Keys.MaxCountOnHands, 3);
    }
    #endregion

    public void IncreaseMaxCountOnHands()
    {
        _maxCountOnHands++;
        PlayerPrefs.SetInt(Keys.MaxCountOnHands, MaxCountOnHands);
    }
}

