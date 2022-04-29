using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < 3; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sequence.Join(cube.transform.DOMove(new Vector3(i, i, 0), 1f).OnStart(() =>
            {
                Debug.Log("Cube " + i + " moved");
            }).SetDelay(0.1f));
        }

        sequence.OnComplete(() =>
        {
            Debug.Log("Complete");
        });
    }
}
