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
    private List<BotController> _bots = new List<BotController>();

    public new List<GameObject> Resources => _resources;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            AddResourceToStorage(player);
        }

        if (other.TryGetComponent(out BotController agent))
        {
            agent.OnObjectEnter(this);
            _bots.Add(agent);

            AllocateResourcesToBots();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out BotController agent))
        {
            agent.OnObjectExit();
            _bots.Remove(agent);
        }
    }
    #endregion
    
    protected void AddResourceToStorage(PlayerController player)
    {
        if (_resources.Count == _positions.Count || player.ResourcesOnHands.Count == 0)
        {
            return;
        }

        foreach (ResourceParams resource in player.ResourcesOnHands.ToList())
        {
            if(_resources.Count >= _positions.Count)
            {
                return;
            }

            if (resource.Resource != CurrentResource)
            {
                continue;
            }

            _resources.Insert(0, resource.Obj);
            resource.Obj.transform.DOMove(_positions[_resources.Count - 1].position, 0.5f);

            player.RemoveResourceOnHands(resource);
        }
            
        AllocateResourcesToBots();
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

    private void AllocateResourcesToBots()
    {
        if (_bots.Count == 0 || _resources.Count == 0)
        {
            return;
        }

        foreach (BotController bot in _bots.ToList())
        {
            foreach(GameObject resource in _resources.ToList())
            {
                _resources.Remove(_resources.First());

                if (_bots.First().AddResourceOnHands(resource))
                { 
                    StartCoroutine(bot.NextTarget());
                    _bots.Remove(bot);
                    
                    return;
                }
            }
        }
    }
}
