using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public class ResourceParams
    {
        public GameObject Obj { get; set; }
        public Resource.ResourceType Resource { get; set; }
    }

    [SerializeField] private Canvases _canvases;
    [SerializeField] private CashData _cashData;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _hands;

    private List<ResourceParams> _resourcesOnHands = new List<ResourceParams>();
    private int _maxCountOnHands = 3;

    public List<ResourceParams> ResourcesOnHands => _resourcesOnHands;
    public GameObject PlayerObj => _playerObj;
    public GameObject Hands => _hands;
    public int MaxCountOnHands => _maxCountOnHands; 
    public CashData CashData => _cashData;

    #region MonoBehaviour
    private void Awake()
    {
        _maxCountOnHands = PlayerPrefs.GetInt(Keys.MaxCountOnHands, 3);
    }
    #endregion

    public void IncreaseMaxCountOnHands()
    {
        _maxCountOnHands++;
        PlayerPrefs.SetInt(Keys.MaxCountOnHands, MaxCountOnHands);
    }
 }
