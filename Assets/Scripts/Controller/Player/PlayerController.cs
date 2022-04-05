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

/*        _animator.SetFloat("Blend", Mathf.Sqrt(Mathf.Abs(_joystick.Vertical - _joystick.Horizontal)));
        Debug.Log(Mathf.Sqrt(Mathf.Abs(_joystick.Vertical - _joystick.Horizontal)));*/

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
        if (ResourcesOnHands.Count >= MaxCountOnHands)
        {
            return true;
        }

        _currentAnimation = Keys.CarryingRunning;
        _animator.SetBool(Keys.CarryingRunning, true);
        _animator.SetBool(Keys.CarryingIdle, true);
        _animator.SetBool(Keys.Running, false);

        // переделать
        float posY = ResourcesOnHands.Count == 0 ? 0 : ResourcesOnHands.Count * obj.transform.localScale.y;
        Debug.Log($"posY {ResourcesOnHands.Count * obj.transform.localScale.y}");
        obj.transform.position = Hands.transform.position + new Vector3(0, posY, 0);
        obj.transform.SetParent(Hands.transform);
        //

        ResourcesOnHands.Add(new Resource.ResourceParams { Obj = obj, Resource = resource });

        return false;
    }
    
    public void RemoveResourceOnHands(Resource.ResourceParams resource)
    {
        if (ResourcesOnHands.Count == 0)
        {
            return;
        }
        
        resource.Obj.transform.SetParent(null);
        ResourcesOnHands.Remove(resource);

        if(ResourcesOnHands.Count == 0)
        {
            _animator.SetBool(Keys.CarryingIdle, false);
            _animator.SetBool(Keys.CarryingRunning, false);
            _animator.SetBool(Keys.Running, true);
            _currentAnimation = Keys.Running;
        }
    }

    public void GiveResources()
    {
        if(ResourcesOnHands.Count == 0)
        {
            _animator.SetBool(Keys.CarryingRunning, false);
            _currentAnimation = Keys.Running;
        }
    }

    public IEnumerator RemoveResourcesOnHands()
    {
        _currentAnimation = Keys.Running;
        _animator.SetBool(Keys.CarryingRunning, false);

        for (int i = 0; i < ResourcesOnHands.Count;)
        {
            Destroy(ResourcesOnHands[i].Obj);
            ResourcesOnHands.RemoveAt(i);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void DestroyResourcesOnHands()
    {
        ResourcesOnHands.Clear();
    }
}
