using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Sprites))]
public class BotBar : MonoBehaviour
{
    [SerializeField] private TMP_Text _demand;

    private Sprites _sprites;
    private Camera _camera;

    #region MonoBehaviour
    private void Awake()
    {
        _sprites = GetComponent<Sprites>();
        _camera = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(_camera.transform);
    }
    #endregion

    public void SetData(Resource.Resources resource, int currentCount, int totalCount)
    {
        _sprites.SetSprite(resource);
        UpdateData(currentCount, totalCount);
    }

    public void UpdateData(int currentCount, int totalCount)
    {
        _demand.text = $"{currentCount}/{totalCount}";
    }
}
