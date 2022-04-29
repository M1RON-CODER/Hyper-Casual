using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ResourceStorage : Resource
{
    [SerializeField] private List<Transform> _resourcePositions = new();

    private List<GameObject> _resources = new();
    private List<Buyer> _AI = new();

    private PlayerController _player;

    public new List<GameObject> Resources => _resources;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = player;
            AddResourceToStorage(player);
        }

        if (other.TryGetComponent(out Buyer AI))
        {
            AI.Stop();
            _AI.Add(AI);

            Sequence sequence = DOTween.Sequence();
            sequence.OnComplete(() => { AllocateResourcesToAI(AI); }).SetDelay(0.25f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = null;
        }

        if (other.TryGetComponent(out Buyer AI))
        {
            AI.Move();
            _AI.Remove(AI);
        }
    }
    #endregion

    protected void AddResourceToStorage(PlayerController? player)
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
        .SetDelay(0.3f);
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
        foreach (Buyer AI in _AI.ToList())
        {
            if (AI.AddResourceOnHands(_resources))
            {
                _AI.Remove(AI);
            }
        }

        AddResourceToStorage(_player);
    }

    private void AllocateResourcesToAI(Buyer AI)
    {
        if(_resources.Count == 0)
        {
            return;
        }   

        if (AI.AddResourceOnHands(_resources))
        {
            _AI.Remove(AI);
        }

        AddResourceToStorage(_player);        
    }
}
