using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private List<Resource.ResourceType> _resourcesOnHands = new List<Resource.ResourceType>();
    private int _maxCountOnHands = 3;

    public List<Resource.ResourceType> ResourcesOnHands { get { return _resourcesOnHands; } set { _resourcesOnHands = value; } }

    private void Start()
    {
        _maxCountOnHands = PlayerPrefs.GetInt("MaxCountOnHands");   
    }

    public void AddResourcesOnHands(Resource.ResourceType resource)
    {
        if(_resourcesOnHands.Count >= _maxCountOnHands)
        {
            return;
        }

        _resourcesOnHands.Add(resource);
    }

    public void IncreaseMaxCountOnHands()
    {
        _maxCountOnHands++;
    }

    public void DestroyResourcesOnHands()
    {
        _resourcesOnHands.Clear();
    }
}
