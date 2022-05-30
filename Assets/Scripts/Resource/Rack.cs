using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rack : Resource, IRack
{
    [SerializeField] private List<Transform> _resourcePositions = new ();
    [SerializeField] private Transform _botPosition;

    private List<GameObject> _resources = new ();
    private List<Buyer> _buyer = new ();
    private PlayerController _player;
    private Assistant _assistan;

    public new List<GameObject> Resources => _resources;
    public Transform BotPosition => _botPosition;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = player;
            AddResourceToStorage(player);
        }

        if (other.TryGetComponent(out Buyer buyer))
        {
            if ((buyer.CurrentTarget == null) || (buyer.CurrentTarget.Resource != CurrentResource))
            {
                return;
            }

            buyer.Stop();
            _buyer.Add(buyer);

            Sequence sequence = DOTween.Sequence();
            sequence.OnComplete(() => { AllocateResourcesBuyer(buyer); }).SetDelay(0.25f);
        }

        if (other.TryGetComponent(out Assistant assistant))
        {
            _assistan = assistant;
            AddResourceToStorage(assistant);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _player = null;
        }
        
        if (other.TryGetComponent(out Assistant assistant))
        {
            _assistan = null;
        }

        if (other.TryGetComponent(out Buyer buyer))
        {
            buyer.Move();
            _buyer.Remove(buyer);
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
        foreach (ResourceBase resource in player.ResourcesOnHands.ToList())
        {
            if (_resources.Count >= _resourcePositions.Count)
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
            resource.Obj.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.3f);
        }

        sequence.OnComplete(() =>
        {
            AllocateResourcesToAI();
        })
        .SetDelay(0.3f);
    }

    protected void AddResourceToStorage(Assistant? assistant)
    {
        if ((_resources.Count == _resourcePositions.Count) || (assistant.ResourcesOnHands.Count == 0))
        {
            return;
        }

        Sequence sequence = DOTween.Sequence();
        foreach (ResourceBase resource in assistant.ResourcesOnHands.ToList())
        {
            if (_resources.Count >= _resourcePositions.Count)
            {
                return;
            }

            if (resource.Resource != CurrentResource)
            {
                continue;
            }

            assistant.RemoveResourceFromHands(resource);
            _resources.Insert(0, resource.Obj);
            resource.Obj.transform.DOMove(_resourcePositions[_resources.Count - 1].position, 0.3f);
        }

        assistant.Move(assistant.ProductionResource);

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
        foreach (Buyer buyer in _buyer.ToList())
        {
            if (buyer.AddResourceOnHands(_resources))
            {
                _buyer.Remove(buyer);
            }
        }

        if(_player != null)
            AddResourceToStorage(_player);
        else
            AddResourceToStorage(_assistan);
    }

    private void AllocateResourcesBuyer(Buyer buyer)
    {
        if (_resources.Count == 0)
        {
            return;
        }

        if (buyer.AddResourceOnHands(_resources))
        {
            _buyer.Remove(buyer);
        }

        if (_player != null)
            AddResourceToStorage(_player);
        else
            AddResourceToStorage(_assistan);
    }
}
