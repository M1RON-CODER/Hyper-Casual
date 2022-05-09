using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ProductionResource : Resource
{
    [SerializeField] private List<Transform> _resourcePositions = new ();
    [SerializeField] private GameObject _resourcePrefab;
    [SerializeField] private float _delayInstatiateResources;
    [Min(0.3f)] [SerializeField] private float _easing = 0.3f;

    private List<GameObject> _resources = new ();
    private PlayerController _playerController;
    private Helper _helper;

    #region MonoBehaviour
    private void Start()
    {
        InstantiateResources();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController playerController))
        {
            _playerController = playerController;

            if (_resources.Count > 0)
            {
                TakeResource(playerController);
            }
        }

        if (other.TryGetComponent(out Helper helper))
        {
            _helper = helper;
            _helper.Stop();

            if (_resources.Count > 0)
            {
                TakeResource(helper);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _playerController = null;
        }

        if (other.TryGetComponent(out Helper helper))
        {
            _helper = null;
        }
    }
    #endregion

    private void TakeResource(PlayerController player)
    {
        foreach (GameObject resource in _resources.ToList())
        {
            if (player.AddResourceOnHands(CurrentResource, resource))
            {
                break;
            }

            _resources.Remove(resource);
        }

        Invoke(nameof(InstantiateResources), _delayInstatiateResources);
    }

    private void TakeResource(Helper helper)
    {
        foreach (GameObject resource in _resources.ToList())
        {
            if (helper.AddResourceOnHands(CurrentResource, resource))
            {
                break;
            }

            _resources.Remove(resource);
        }

        helper.Move(helper.ResourceStorage);
        Invoke(nameof(InstantiateResources), _delayInstatiateResources);
    }

    private void InstantiateResources()
    {
        int count = _resourcePositions.Count - _resources.Count;

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < count; i++)
        {
            GameObject resource = Instantiate(_resourcePrefab, _resourcePositions[i]);
            resource.name = i.ToString();
            _resources.Add(resource);
            resource.transform.SetParent(_resourcePositions[i]);

            Vector3 endScale = resource.transform.localScale;
            resource.transform.localScale = Vector3.zero;
            resource.transform.DOScale(endScale, _easing);
        }

        sequence.OnComplete(() =>
        {
            if (_playerController)
            {
                TakeResource(_playerController);
            }
            else if (_helper)
            {
                TakeResource(_helper);
            }
        }).SetDelay(_easing);
    }
}
