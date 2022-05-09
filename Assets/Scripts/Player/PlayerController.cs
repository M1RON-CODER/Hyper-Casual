using DG.Tweening;
using MoreMountains.NiceVibrations;
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

    public override bool AddResourceOnHands(Resource.Resources resource, GameObject resourceObject)
    {
        if (ResourcesOnHands.Count >= MaxCountOnHands)
        {
            return true;
        }

        Vector3 position = GetPositionForResourceOnHands();
        resourceObject.transform.SetParent(Hands);
        resourceObject.transform.DOLocalMove(position, 0.2f);

        ResourcesOnHands.Insert(0, new Resource.ResourceBase { Obj = resourceObject, Resource = resource });

        AnimationAdjustment();
        MMVibrationManager.Haptic(HapticTypes.LightImpact, false, true, this);

        return false;
    }

    public override void RemoveResourceFromHands(Resource.ResourceBase resource)
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
        foreach (Resource.ResourceBase resource in ResourcesOnHands)
        {
            position.y += resource.Obj.transform.localScale.y;
        }

        return position;
    }

    public override void AnimationAdjustment()
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
