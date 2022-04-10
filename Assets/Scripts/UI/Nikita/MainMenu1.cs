using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu1 : MonoBehaviour
{
    private Canvases1 _canvases;

    public void Init(Canvases1 canvases)
    {
        _canvases = canvases;
    }

    public void OnClickSettings()
    {
        Hide();
        _canvases.Menu.Settings.Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

