using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player
{
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private int _speed;

    private string _currentAnimation = Keys.Running;

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
        AnimationAdjustment();

        if (ResourcesOnHands.Count >= MaxCountOnHands)
        {
            return true;
        }
        
        Vector3 position = GetPositionForResourceOnHands();
        resourceObj.transform.SetParent(Hands.transform);
        resourceObj.transform.DOLocalMove(position, 0.2f);

        ResourcesOnHands.Insert(0, (new Resource.ResourceParams { Obj = resourceObj, Resource = resource }));

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
            position.y += resource.Obj.transform.localScale.x;
        }

        return position;    
    }

    private void AnimationAdjustment()
    {
        if (ResourcesOnHands.Count > 0)
        {
            Animator.SetBool(Keys.CarryingIdle, true);
            Animator.SetBool(Keys.CarryingRunning, true);

            Animator.SetBool(Keys.Idle, false);
            Animator.SetBool(Keys.Running, false);

            _currentAnimation = Keys.CarryingRunning;
        }
        else
        {
            Animator.SetBool(Keys.Idle, true);
            Animator.SetBool(Keys.Running, true);

            Animator.SetBool(Keys.CarryingIdle, false);
            Animator.SetBool(Keys.CarryingRunning, false);

            _currentAnimation = Keys.Running;
        }
    }
}
