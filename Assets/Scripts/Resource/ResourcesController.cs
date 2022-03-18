using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesController : Resource
{
    [SerializeField] private List<Transform> _positionResources = new List<Transform>();
    [SerializeField] private GameObject _objResource;
    [SerializeField] private GameObject _zone;
    [SerializeField] private float _delayInstatiate;

    private List<GameObject> _resources = new List<GameObject>();
    private Vector3 _defaultScale;
    private float _size = 1.2f;
    private float _easing = 0.3f;
    private bool _onTriggerEnter = false; 

    private void Start()
    {
        _defaultScale = _zone.transform.localScale;
        StartCoroutine(InstaiateResources());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            StartCoroutine(ChangeOfSize(_zone, _zone.transform.localScale * _size, _easing));
            StartCoroutine(TakeResource());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(ChangeOfSize(_zone, _defaultScale, _easing));
    }

    private IEnumerator TakeResource()
    {
        foreach(GameObject resource in _resources)
        {
            Destroy(resource);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator ChangeOfSize(GameObject obj, Vector3 endScale, float easing)
    {
        float t = 0;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / easing;
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, endScale, t); 
            yield return null;
        }
    }

    private IEnumerator InstaiateResources()
    {
        while (true)
        {
            yield return new WaitForSeconds(_delayInstatiate);
            for(int i = _resources.Count; i < _positionResources.Count; i++)
            {
                GameObject resource = Instantiate(_objResource, _positionResources[i]);
                
                Vector3 endScale = resource.transform.localScale;
                resource.transform.localScale *= 0;
                StartCoroutine(ChangeOfSize(resource, endScale, _easing));

                _resources.Add(resource);
            }
        }
    }
}
