using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceStorage : Resource
{
    [SerializeField] private List<Transform> _positions = new List<Transform>();

    private List<GameObject> _resources = new List<GameObject>();
    private List<AIController> _AI = new List<AIController>();

    public new List<GameObject> Resources => _resources;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AIController AI))
        {
            AI.Stop();
            _AI.Add(AI);

            AllocateResourcesToAI(AI);
        }
        
        if (other.TryGetComponent(out PlayerController player))
        {
            AddResourceToStorage(player);        
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
        if ((_resources.Count == _positions.Count) || (player.ResourcesInHands.Count == 0))
        {
            return;
        }

        foreach (ResourceParams resource in player.ResourcesInHands.ToList())
        {
            if(_resources.Count >= _positions.Count)
            {
                return;
            }

            if (resource.Resource != CurrentResource)
            {
                continue;
            }

            if (_AI.Count > 0)
            {
                PassResourcesToAIs(player);
                AddResourceToStorage(player);
                return;
            }
            
            _resources.Insert(0, resource.Obj);
            
            if(_AI.Count == 0)
            {
                resource.Obj.transform.DOMove(_positions[_resources.Count - 1].position, 0.3f);
            }

            player.RemoveResourceOnHands(resource);
        }
    }

    public GameObject RemoveResource()
    {
        if (_resources.Count > 0)
        {
            GameObject resource = _resources.Last();
            _resources.Remove(_resources.Last());

            return resource;
        }
        
        return null;
    }
    
    private void AllocateResourcesToAI(AIController AI)
    {      
        if (_resources.Count == 0)
        {
            return;
        }
        
        foreach(GameObject resource in _resources.ToList())
        {
            _resources.Remove(_resources.First());

            if (AI.AddResourceToHands(resource))
            {
                AI.NextTarget();
                _AI.Remove(AI);
                
                return;
            }
        }   
    }

    private void PassResourcesToAIs(PlayerController player)
    {      
        foreach (AIController AI in _AI.ToList())
        {
            foreach (ResourceParams resource in player.ResourcesInHands.ToList())
            {
                if(resource.Resource != CurrentResource)
                {
                    continue;
                }

                player.RemoveResourceOnHands(resource);

                if (AI.AddResourceToHands(resource.Obj))
                {
                    AI.NextTarget();
                    _AI.Remove(AI);

                    return;
                }
            }
        }
    }
}
