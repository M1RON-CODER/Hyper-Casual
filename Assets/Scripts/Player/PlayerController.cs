using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player
{
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private int _speed;

    private string _currentAnimation = Keys.Animation.Running.ToString();
    private bool _isEntry = false;

    public bool IsEntry => _isEntry;

    #region MonoBehaviour

    public void FixedUpdate()
    {
        Animator.SetBool(_currentAnimation, _joystick.IsTouch);

        Vector3 direction = Vector3.forward * _joystick.Vertical + Vector3.right * _joystick.Horizontal;

        Rigidbody.MovePosition(transform.position + direction * _speed * Time.deltaTime);

        if(direction != Vector3.zero)
        {
            Vector3 relativePos = PlayerObject.transform.position;
            relativePos.Set(_joystick.Horizontal, 0, _joystick.Vertical);
            PlayerObject.transform.rotation = Quaternion.LookRotation(relativePos);
        }
    }
    #endregion

    public bool AddResourcesOnHands(Resource.Resources resource, GameObject resourceObj)
    {
        if (ResourcesOnHands.Count >= MaxCountOnHands)
        {
            return true;
        }
        
        Vector3 position = GetPositionForResourceOnHands();
        resourceObj.transform.SetParent(Hands);
        resourceObj.transform.DOLocalMove(position, 0.2f);

        ResourcesOnHands.Insert(0, (new Resource.ResourceParams { Obj = resourceObj, Resource = resource }));

        AnimationAdjustment();
        MMVibrationManager.Haptic(HapticTypes.LightImpact, false, true, this);

        return false;
    }
    
    public void RemoveResourceFromHands(Resource.ResourceParams resource)
    {
        if (ResourcesOnHands.Count == 0)
        {
            return;
        }
        
        resource.Obj.transform.SetParent(null);
        ResourcesOnHands.Remove(resource);

        AnimationAdjustment();
        MMVibrationManager.Haptic(HapticTypes.LightImpact, false, true, this);
    }

    public void Enter()
    {
        _isEntry = true;
    }

    public void Exit()
    {
        _isEntry = false;
    }
    
    private Vector3 GetPositionForResourceOnHands()
    {
        if (ResourcesOnHands.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 position = Vector3.zero;
        foreach (Resource.ResourceParams resource in ResourcesOnHands)
        {
            position.y += resource.Obj.transform.localScale.y;
        }

        return position;    
    }

    private void AnimationAdjustment()
    {
        if (ResourcesOnHands.Count > 0)
        {
            Animator.SetBool(Keys.Animation.CarryingIdle.ToString(), true);
            Animator.SetBool(Keys.Animation.CarryingRunning.ToString(), true);

            Animator.SetBool(Keys.Animation.Idle.ToString(), false);
            Animator.SetBool(Keys.Animation.Running.ToString(), false);
            
            _currentAnimation = Keys.Animation.CarryingRunning.ToString();
        }
        else
        {
            Animator.SetBool(Keys.Animation.Idle.ToString(), true);
            Animator.SetBool(Keys.Animation.Running.ToString(), true);

            Animator.SetBool(Keys.Animation.CarryingIdle.ToString(), false);
            Animator.SetBool(Keys.Animation.CarryingRunning.ToString(), false);

            _currentAnimation = Keys.Animation.Running.ToString();
        }
    }
}
