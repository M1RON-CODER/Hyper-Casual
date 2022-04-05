using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Zone : MonoBehaviour
{
    private Vector3 _defaultScale;
    private float _easing = 0.3f;
    private float _zoneScale = 1.2f;

    private void Start()
    {
        _defaultScale = transform.transform.localScale;
    }

    public IEnumerator ChangeOfScale(bool isDefaultScale = false)
    {
        Vector3 endScale = _defaultScale * _zoneScale;
        float t = 0;

        if (isDefaultScale)
        {
            endScale = _defaultScale;
        }

        while (t <= 1.0f)
        {
            t += Time.deltaTime / _easing;
            transform.localScale = Vector3.Lerp(transform.localScale, endScale, t);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(ChangeOfScale());
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(ChangeOfScale(true));
    }
}
