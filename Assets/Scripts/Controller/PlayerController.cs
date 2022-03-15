using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public DynamicJoystick joystick;
    public float speed;

    public void Update()
    {
        float degrees = ((Mathf.Atan2(joystick.Vertical, joystick.Horizontal) + 2f * Mathf.PI) * 180f / Mathf.PI) % 360f;

        Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
        //Debug.Log(degrees);

        transform.Translate(direction * speed * Time.deltaTime);
        //transform.rotation = Quaternion.Euler(0, degrees, 0);
    }
}
