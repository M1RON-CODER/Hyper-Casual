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
        if (other.TryGetComponent(out BotController bot))
        {
            bot.OnObjectEnter(this);
            _bots.Add(bot);

            AllocateResourcesToBot(bot);
        }
        
        if (other.TryGetComponent(out PlayerController player))
        {
            AddResourceToStorage(player);        
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

            if (_bots.Count > 0)
            {
                AllocateResourcesToBots(player);
                AddResourceToStorage(player);
                return;
            }
            
            _resources.Insert(0, resource.Obj);
            if(_bots.Count == 0)
                resource.Obj.transform.DOMove(_positions[_resources.Count - 1].position, 0.5f);

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
    
    private void AllocateResourcesToBot(BotController bot)
    {      
        if (_resources.Count == 0)
        {
            return;
        }
        
        foreach(GameObject resource in _resources.ToList())
        {
            _resources.Remove(_resources.First());

            if (bot.AddResourceInHands(resource))
            { 
                StartCoroutine(bot.NextTarget());
                _bots.Remove(bot);
                    
                return;
            }
        }   
    }

    private void AllocateResourcesToBots(PlayerController player)
    {      
        foreach (BotController bot in _bots.ToList())
        {
            foreach (ResourceParams resource in player.ResourcesOnHands.ToList())
            {
                if(resource.Resource != CurrentResource)
                {
                    continue;
                }
                
                player.ResourcesOnHands.Remove(resource);

                if (bot.AddResourceInHands(resource.Obj))
                {
                    StartCoroutine(bot.NextTarget());
                    _bots.Remove(bot);

                    return;
                }
            }
        }
    }
}
