using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesController : Resource
{
    [SerializeField] private GameObject _zone;

    private Vector3 _defaultScale;
    private float size = 1.5f;

    private void Start()
    {
        _defaultScale = _zone.transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {

        }
        StartCoroutine(ChangeOfSize(_zone.transform.localScale * size));
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(ChangeOfSize(_defaultScale));
    }

    private IEnumerator ChangeOfSize(Vector3 endScale)
    {
        float t = 0, easing = 0.3f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / easing;
            _zone.transform.localScale = Vector3.Lerp(_zone.transform.localScale, endScale, t); 
            yield return null;
        }
    }
}
