using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprites : MonoBehaviour
{
    [SerializeField] private GameObject _firTree;
    [SerializeField] private GameObject _cashRegister;
    
    private GameObject _currentSprite;

    public void SetSprite(Resource.Resources resource)
    {
        if(_currentSprite != null)
        {
            Hide();
        }

        switch(resource)
        {
            case Resource.Resources.FirTree:
                _currentSprite = _firTree;
                break;
            default:
                break;
        }

        Show();
    }

    public void SetSprite(Building.Buildings building)
    {
        if (_currentSprite != null)
        {
            Hide();
        }

        switch (building)
        {
            case Building.Buildings.CashRegister:
                _currentSprite = _cashRegister;
                break;
            default:
                break;
        }

        Show();
    }

    private void Show()
    {
        _currentSprite.gameObject.SetActive(true);
    }

    private void Hide()
    {
        _currentSprite.gameObject.SetActive(false);
    }
}
