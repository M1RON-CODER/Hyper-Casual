using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private List<Transform> _positions = new List<Transform>();
    [SerializeField] private Animator _animator;

    public List<Transform> Positions => _positions;

    #region MonoBeHahaviour
    private void Awake()
    {
           
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
