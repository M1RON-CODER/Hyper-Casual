using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcesController : Resource
{
    [SerializeField] private List<Transform> _positionResources = new List<Transform>();
    [SerializeField] private GameObject _objResource;
    [SerializeField] private float _delayInstatiateResources;
    [SerializeField] private float _easing = 0.3f;

    private List<GameObject> _resources = new List<GameObject>();
    private PlayerController _playerController;
    private bool _isEnter = false;

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
            _isEnter = true;

            if(_resources.Count > 0)
                StartCoroutine(TakeResource());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            _isEnter = false;
        }
    }
    #endregion
    private IEnumerator TakeResource()
    {
        for(int i = 0; i < _resources.Count;)
        {
            if (_playerController.AddResourcesOnHands(CurrentResource, _resources[i]))
            {
                Invoke(nameof(InstantiateResources), _delayInstatiateResources);
                yield break;
            }

            _resources.RemoveAt(i);
         
            yield return new WaitForSeconds(0.05f);
        }

        Invoke(nameof(InstantiateResources), _delayInstatiateResources);
    }

    private IEnumerator ChangeOfScale(GameObject obj, Vector3 endScale, float easing)
    {
        float t = 0;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / easing;
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, endScale, t);
            yield return null;
        }
    }

    private void InstantiateResources()
    {
        int count = _positionResources.Count - _resources.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject resource = Instantiate(_objResource, _positionResources[i]);
            _resources.Insert(i, resource);

            Vector3 endScale = resource.transform.localScale;
            resource.transform.localScale *= 0;
            StartCoroutine(ChangeOfScale(resource, endScale, _easing));
        }

        if (_isEnter)
        {
            StartCoroutine(TakeResource());
        }
    }
}
