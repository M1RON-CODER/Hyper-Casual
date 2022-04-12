using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Box : MonoBehaviour
{
    [SerializeField] private List<Transform> _positions = new List<Transform>();

    private Animator _animator;

    public List<Transform> Positions => _positions;

    #region MonoBeHahaviour
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    #endregion

    public void OpenBox()
    {
        _animator.SetBool(Keys.Opening, true);
    }

    public void CloseBox()
    {
        _animator.SetBool(Keys.Opening, false);
    }
}
