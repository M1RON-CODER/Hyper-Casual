using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    private Vector3 _offset;

    #region MonoBehaviour
    private void Start()
    {
        _offset = transform.position;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Slerp(transform.position, _target.transform.position + _offset, 0.2f);
    }
    #endregion
}
