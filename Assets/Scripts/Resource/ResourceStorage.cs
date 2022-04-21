using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceStorage : Resource
{
    [SerializeField] private List<Transform> _resourcePositions = new();

    private List<GameObject> _resources = new();
    private List<AIController> _AI = new();

    public new List<GameObject> Resources => _resources;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            AddResourceToStorage(player);
        }
        else if (other.TryGetComponent(out AIController AI))
        {
            AI.Stop();
            _AI.Add(AI);

            AllocateResourcesToAI(AI);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out AIController AI))
        {
            AI.Move();
            _AI.Remove(AI);
        }
    }
    #endregion
    
    protected void AddResourceToStorage(PlayerController player)
    {
        if ((_resources.Count == _resourcePositions.Count) || (player.ResourcesOnHands.Count == 0))
        {
            throw new Exception("No space for resources");
        }

        Sequence sequence = DOTween.Sequence();
        foreach (ResourceParams resource in player.ResourcesOnHands.ToList())
        {
            if(_resources.Count >= _resourcePositions.Count)
            {
                return;
            }

            if (resource.Resource != CurrentResource)
            {
                continue;
            }
            
            _resources.Insert(0, resource.Obj);
            sequence.Append(resource.Obj.transform.DOMove(_resourcePositions[0].position, 0.5f));

            player.RemoveResourceFromHands(resource);
        }

        sequence.OnComplete(() => 
        {
            AllocateResourcesToAI();
        });
    }

    public GameObject RemoveResourceFromStorage()
    {
        if (_resources.Count > 0)
        {
            GameObject resource = _resources.Last();
            _resources.Remove(_resources.Last());

            return resource;
        }
        
        return null;
    }
    
    private void AllocateResourcesToAI()
    {
        foreach (AIController AI in _AI.ToList())
        {
            foreach (GameObject resource in _resources.ToList())
            {
                _resources.Remove(resource);
                if (AI.AddResourceToHands(resource))
                {
                    _AI.Remove(AI);
                    return;
                }
            }
        }
    }

    private void AllocateResourcesToAI(AIController AI)
    {
        foreach (GameObject resource in _resources.ToList())
        {
            _resources.Remove(resource);
            if (AI.AddResourceToHands(resource))
            {
                _AI.Remove(AI);
                return;
            }
        }
    }
}
