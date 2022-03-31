using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : Player
{
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private int _speed;

    private Animator _animator;
    private string _currentAnimation = Keys.Running;

    public string CurrentAnimation => _currentAnimation;

    private void Start()
    {
        _animator = GetComponent<Animator>();   
    }

    public void Update()
    {
        _animator.SetBool(_currentAnimation, _joystick.IsTouch);

        Vector3 direction = Vector3.forward * _joystick.Vertical + Vector3.right * _joystick.Horizontal;

        transform.Translate(direction * _speed * Time.deltaTime);

        if(direction != Vector3.zero)
        {
            Vector3 relativePos = PlayerObj.transform.position;
            relativePos.Set(_joystick.Horizontal, 0, _joystick.Vertical);
            PlayerObj.transform.rotation = Quaternion.LookRotation(relativePos);
        }
    }

    public bool AddResourcesOnHands(Resource.ResourceType resource, GameObject obj)
    {
        if (ResourcesOnHands.Count >= MaxCountOnHands)
        {
            return true;
        }

        _currentAnimation = Keys.Carrying;
        _animator.SetBool(Keys.Carrying, true);
        _animator.SetBool(Keys.Running, false);

        // переделать
        float posy = ResourcesOnHands.Count == 0 ? 0 : ResourcesOnHands.Count * 0.3f;
        obj.transform.position = Hands.transform.position + new Vector3(0, posy, 0);
        obj.transform.SetParent(Hands.transform);
        //

        ResourcesOnHands.Add(new ResourceParams { Obj = obj, Resource = resource });

        return false;
    }

    public void GiveResources()
    {
        if(ResourcesOnHands.Count == 0)
        {
            _animator.SetBool(Keys.Carrying, false);
            _currentAnimation = Keys.Running;
        }
    }

    public IEnumerator RemoveResourcesOnHands()
    {
        _currentAnimation = Keys.Running;
        _animator.SetBool(Keys.Carrying, false);

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
