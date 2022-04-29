using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TrashCan : MonoBehaviour
{
    [Min(0.1f)] [SerializeField] private float _duration = 0.2f;

    private Animator _animator;
    private bool _isEntry;

    #region MonoBehaviour
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _isEntry = true;
            StartCoroutine(RemoveResourcesInHands(player));

            _animator.SetBool(Keys.Animation.Opening.ToString(), true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _isEntry = false;

            _animator.SetBool(Keys.Animation.Opening.ToString(), false);
        }
    }
    #endregion

    private IEnumerator RemoveResourcesInHands(PlayerController player)
    {
        foreach (Resource.ResourceParams resource in player.ResourcesOnHands.ToList())
        {
            if (_isEntry)
            {
                player.RemoveResourceFromHands(resource);
                resource.Obj.transform.DOMove(transform.position, _duration);
                Destroy(resource.Obj, _duration);
            }
            
            if (!_isEntry)
            {
                yield break;
            }

            yield return new WaitForSeconds(_duration);
        }

        _animator.SetBool(Keys.Animation.Opening.ToString(), false);
    }
}
