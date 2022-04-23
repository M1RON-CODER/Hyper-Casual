using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerResources: Resource
{
    [SerializeField] private List<Transform> _resourcePositions = new();
    [SerializeField] private GameObject _resourcePrefab;
    [SerializeField] private float _delayInstatiateResources;
    [Min(0.3f)] [SerializeField] private float _easing = 0.3f;

    private List<GameObject> _resources = new();
    private PlayerController _playerController;
    private bool _isPlayerEnter;

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
            _isPlayerEnter = true;

            if(_resources.Count > 0)
            {
                TakeResource();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            _isPlayerEnter = false;
        }
    }
    #endregion

    private void TakeResource()
    {
        foreach (GameObject resource in _resources.ToList())
        {
            
            if (_playerController.AddResourcesOnHands(CurrentResource, resource))
            {
                return;
            }

            _resources.Remove(resource);
        }

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
            if (_isPlayerEnter)
            {
                TakeResource();
            }
        }).SetDelay(_easing);
    }
}
