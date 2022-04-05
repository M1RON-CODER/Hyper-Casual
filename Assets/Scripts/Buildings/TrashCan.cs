using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            StartCoroutine(other.GetComponent<PlayerController>().RemoveResourcesOnHands());
        }
    }
}
