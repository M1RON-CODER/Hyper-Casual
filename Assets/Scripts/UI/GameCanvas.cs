using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] TMP_Text _cash;

    public void RefreshCash(int cash)
    {
        _cash.text = cash.ToString();
    }
}
