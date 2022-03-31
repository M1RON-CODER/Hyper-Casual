using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvases : MonoBehaviour
{
    [SerializeField] private GameCanvas _gameCanvas;

    public GameCanvas GameCanvas => _gameCanvas;
}
