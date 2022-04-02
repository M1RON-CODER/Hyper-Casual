using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashData : MonoBehaviour
{
    [SerializeField] private Canvases _canvases;
    private int _cash;

    #region MonoBehaviour
    private void Awake()
    {
        _cash = PlayerPrefs.GetInt(Keys.Cash);
        RefreshCash();
    }
    #endregion

    public int Cash => _cash;

    public void DepositCash(int cash)
    {
        _cash += cash;
        RefreshCash();
        SaveCash();
    }

    public void WithdrawCash(int cash)
    {
        _cash -= cash;
        RefreshCash();
        SaveCash();
    }

    private void RefreshCash()
    {
        _canvases.GameCanvas.RefreshCash(_cash);
    }

    private void SaveCash()
    {
        PlayerPrefs.SetInt(Keys.Cash, _cash);
    }
}
