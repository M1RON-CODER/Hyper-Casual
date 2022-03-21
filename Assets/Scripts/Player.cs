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

    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _hands;

    private List<ResourceParams> _resourcesOnHands = new List<ResourceParams>();
    private int _maxCountOnHands = 3;

    public List<ResourceParams> ResourcesOnHands { get { return _resourcesOnHands; } set { _resourcesOnHands = value; } }
    public GameObject PlayerObj { get { return _playerObj; } }
    public GameObject Hands { get { return _hands; } }
    public int MaxCountOnHands { get { return _maxCountOnHands; } }

    private void Awake()
    {
        _maxCountOnHands = PlayerPrefs.GetInt(Keys.MaxCountOnHands);   
    }

    public virtual void IncreaseMaxCountOnHands()
    {
        _maxCountOnHands++;
        PlayerPrefs.SetInt(Keys.MaxCountOnHands, MaxCountOnHands);
    }
}
