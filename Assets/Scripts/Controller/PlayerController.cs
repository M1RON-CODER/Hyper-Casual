using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private int _speed;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();   
    }

    public void Update()
    {
        _animator.SetBool(AnimatorKeys.Running, _joystick.IsTouch);
        
        // float degrees = ((Mathf.Atan2(joystick.Vertical, joystick.Horizontal) + 2f * Mathf.PI) * 180f / Mathf.PI) % 360f;

        Vector3 direction = Vector3.forward * _joystick.Vertical + Vector3.right * _joystick.Horizontal;

        transform.Translate(direction * _speed * Time.deltaTime);

        Vector3 relativePos = _playerObj.transform.position;
        relativePos.Set(_joystick.Horizontal, 0, _joystick.Vertical);
        _playerObj.transform.rotation = Quaternion.LookRotation(relativePos);
    }
}
