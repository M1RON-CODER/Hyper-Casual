using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Sprites))]
public class AIBar : MonoBehaviour
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

    public void SetData(Resource.Resources resource)
    {
        _sprites.SetSprite(resource);
    }

    public void SetData(Building.Buildings building)
    {
        _sprites.SetSprite(building);
        _demand.gameObject.SetActive(false);    
    }

    public void SetData(Resource.Resources resource, int currentCount, int totalCount)
    {
        _sprites.SetSprite(resource);
        RefreshData(currentCount, totalCount);
    }

    public void RefreshData(int currentCount, int totalCount)
    {
        _demand.text = $"{currentCount}/{totalCount}";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void RefreshData()
    {
        _demand.text = "";
    }
}
