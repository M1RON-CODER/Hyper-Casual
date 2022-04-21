using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [Min(0.2f)] [SerializeField] private float _duration = 0.2f;

    private bool _isEntry;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _isEntry = true;
            StartCoroutine(RemoveResourcesInHands(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _isEntry = false;
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
    }
}
