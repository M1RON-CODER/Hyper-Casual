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
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _hands;

    private List<ResourceParams> _resourcesOnHands = new List<ResourceParams>();
    private int _maxCountOnHands = 3;
    private int _cash;

    public List<ResourceParams> ResourcesOnHands => _resourcesOnHands;
    public GameObject PlayerObj => _playerObj;
    public GameObject Hands => _hands;
    public int MaxCountOnHands => _maxCountOnHands; 
    public int Cash => _cash;

    private void Awake()
    {
        _cash = PlayerPrefs.GetInt(Keys.Cash);
        _maxCountOnHands = PlayerPrefs.GetInt(Keys.MaxCountOnHands, 3);

        RefreshCash();
    }

    public virtual void IncreaseMaxCountOnHands()
    {
        _maxCountOnHands++;
        PlayerPrefs.SetInt(Keys.MaxCountOnHands, MaxCountOnHands);
    }

    public void DepositCash(int cash)
    {
        _cash += cash;
        RefreshCash();
        SaveCash();
    }

    public void WithdrawCash(int cash)
    {
        _cash -= cash;
        RefreshCash();
        SaveCash();
    }

    private void RefreshCash()
    {
        _canvases.GameCanvas.RefreshCash(_cash);
    }

    private void SaveCash()
    {
        PlayerPrefs.SetInt(Keys.Cash, _cash);
    }
 }
