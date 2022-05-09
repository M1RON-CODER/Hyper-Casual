using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
    private Animator _animator;

    #region MonoBehaviour
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnTriggerStay(Collider other)
    {
        _animator.SetBool(Keys.Animation.Opening.ToString(), true);
    }

    private void OnTriggerExit(Collider other)
    {
        _animator.SetBool(Keys.Animation.Opening.ToString(), false);
    }

    #endregion
}
