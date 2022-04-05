using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceStorage : Resource
{
    [SerializeField] private List<Transform> _positions = new List<Transform>();

    private List<GameObject> _resources = new List<GameObject>();
    private int _currentCountResources = 0;

    public new List<GameObject> Resources => _resources;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            AddResource(player);
        }

        if (other.TryGetComponent(out BotController agent))
        {
            agent.OnObjectEnter(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out BotController agent))
        {
            agent.OnObjectExit();
        }
    }
    #endregion
    
    protected void AddResource(PlayerController player)
    {
        if (_currentCountResources < _positions.Count && player.ResourcesOnHands.Count > 0)
        {
            for (int i = player.ResourcesOnHands.Count - 1; i >= 0; i--)
            {
                if (player.ResourcesOnHands[i].Resource == CurrentResource && _currentCountResources < _positions.Count)
                {
                    _resources.Add(player.ResourcesOnHands[i].Obj);
                    player.ResourcesOnHands[i].Obj.transform.DOMove(_positions[_currentCountResources].position, 0.5f);
                    _currentCountResources++;

                    player.RemoveResourceOnHands(player.ResourcesOnHands[i]);
                }
            }
        }
    }

    public GameObject RemoveResource(GameObject hands)
    {
        if (_resources.Count > 0)
        {
            GameObject resource = _resources.Last();
            resource.transform.DOMove(hands.transform.position, 0.5f);
            _resources.Remove(_resources.Last());
            _currentCountResources--;

            return resource;
        }
        
        return null;
    }
}
