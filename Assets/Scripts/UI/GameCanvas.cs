using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] TMP_Text _cash;
    [SerializeField] GameObject _cashPanel;

    public TMP_Text Cash => _cash;
    public GameObject CashPanel => _cashPanel;

    public void RefreshCash(int cash)
    {
        _cash.text = cash.ToString();
    }
}
