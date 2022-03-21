using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float speed;

    private Vector3 _offset;

    private void Start()
    {
        _offset = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Slerp(player.transform.position + _offset, player.transform.position + _offset, 1);
    }
}
