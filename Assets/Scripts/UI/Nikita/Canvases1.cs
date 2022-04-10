using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvases1 : MonoBehaviour
{
    [SerializeField] private Menu1 _menu;

    public Menu1 Menu => _menu;

    private void Awake()
    {
        Init();
    }

    public void Init() 
    {
        _menu.Init(this);
    }
}
