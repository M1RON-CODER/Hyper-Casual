using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprites : MonoBehaviour
{
    [SerializeField] private GameObject _firTree;

    public GameObject FirTree => _firTree;

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

    private void Show()
    {
        _currentSprite.gameObject.SetActive(true);
    }

    private void Hide()
    {
        _currentSprite.gameObject.SetActive(false);
    }
}
