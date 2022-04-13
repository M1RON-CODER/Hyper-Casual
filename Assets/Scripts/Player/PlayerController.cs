using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : Player
{
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private int _speed;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private string _currentAnimation = Keys.Running;

    public string CurrentAnimation => _currentAnimation;

    #region MonoBehaviour
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();   
    }

    public void FixedUpdate()
    {
        _animator.SetBool(_currentAnimation, _joystick.IsTouch);

        Vector3 direction = Vector3.forward * _joystick.Vertical + Vector3.right * _joystick.Horizontal;

        _rigidbody.MovePosition(transform.position + direction * _speed * Time.deltaTime);

        if(direction != Vector3.zero)
        {
            Vector3 relativePos = PlayerObj.transform.position;
            relativePos.Set(_joystick.Horizontal, 0, _joystick.Vertical);
            PlayerObj.transform.rotation = Quaternion.LookRotation(relativePos);
        }
    }
    #endregion

    public bool AddResourcesOnHands(Resource.Resources resource, GameObject obj)
    {
        if (ResourcesInHands.Count >= MaxCountOnHands)
        {
            return true;
        }
        
        Vector3 position = GetPositionForResourceInHands();
        obj.transform.SetParent(Hands.transform);
        obj.transform.DOLocalMove(position, 0.2f);

        ResourcesInHands.Insert(0, (new Resource.ResourceParams { Obj = obj, Resource = resource }));

        SettingsAnimation();

        return false;
    }
    
    public void RemoveResourceOnHands(Resource.ResourceParams resource)
    {
        if (ResourcesInHands.Count == 0)
        {
            return;
        }
        
        resource.Obj.transform.SetParent(null);
        ResourcesInHands.Remove(resource);

        SettingsAnimation();
    }

    private Vector3 GetPositionForResourceInHands()
    {
        if (ResourcesInHands.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 position = Vector3.zero;
        
        foreach (Resource.ResourceParams resource in ResourcesInHands)
        {
            position.y += resource.Obj.transform.localScale.x;
        }

        return position;    
    }

    private void SettingsAnimation()
    {
        if (ResourcesInHands.Count > 0)
        {
            _animator.SetBool(Keys.CarryingIdle, true);
            _animator.SetBool(Keys.CarryingRunning, true);

            _animator.SetBool(Keys.Idle, false);
            _animator.SetBool(Keys.Running, false);

            _currentAnimation = Keys.CarryingRunning;
        }
        else
        {
            _animator.SetBool(Keys.Idle, true);
            _animator.SetBool(Keys.Running, true);

            _animator.SetBool(Keys.CarryingIdle, false);
            _animator.SetBool(Keys.CarryingRunning, false);

            _currentAnimation = Keys.Running;
        }
    }
}
