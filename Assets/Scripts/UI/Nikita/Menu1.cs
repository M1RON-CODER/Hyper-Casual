using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu1 : MonoBehaviour
{
    [SerializeField] private MainMenu1 _mainMenu;
    [SerializeField] private Settings1 _settings;

    public MainMenu1 MainMenu => _mainMenu;
    public Settings1 Settings => _settings;

    private Canvases1 _canvases;

    public void Init(Canvases1 canvases)
    {
        _canvases = canvases;
        _mainMenu.Init(canvases);
        _settings.Init(canvases);
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
