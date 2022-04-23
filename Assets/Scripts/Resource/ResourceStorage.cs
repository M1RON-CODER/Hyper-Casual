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
        
        if (other.TryGetComponent(out AIController AI))
        {
            AI.Stop();
            _AI.Add(AI);

            Sequence sequence = DOTween.Sequence();
            sequence.OnComplete(() => { AllocateResourcesToAI(AI); }).SetDelay(0.5f);
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
            return;
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
            
            player.RemoveResourceFromHands(resource);
            _resources.Insert(0, resource.Obj);
            resource.Obj.transform.DOMove(_resourcePositions[_resources.Count - 1].position, 0.3f);

        }

        sequence.OnComplete(() => 
        {
            AllocateResourcesToAI();
        })
        .SetDelay(0.35f);
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
            AI.AddResourceOnHands(_resources);
            _AI.Remove(AI);
        }
    }

    private void AllocateResourcesToAI(AIController AI)
    {
        if(_resources.Count == 0)
        {
            return;
        }   

        if (AI.AddResourceOnHands(_resources))
        {
            _AI.Remove(AI);
        }
    }
}
