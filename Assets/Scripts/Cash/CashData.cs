using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashData : MonoBehaviour
{
    [SerializeField] private Canvases _canvases;
    [SerializeField] private int _cash;

    #region MonoBehaviour
    private void OnValidate()
    {
        SaveCash();
    }
    private void Awake()
    {
        _cash = PlayerPrefs.GetInt(Keys.PlayerPrefs.Cash.ToString());
        RefreshCash();
    }
    #endregion

    public int Cash => _cash;

    public void AddCash(int cash)
    {
        _cash += cash;
        _canvases.GameCanvas.Cash.transform.DORewind();
        _canvases.GameCanvas.Cash.transform.DOPunchScale(new Vector3(0.6f, 0.6f, 0.6f), 0.3f);
        
        SaveCash();
    }

    public void WithdrawCash(int cash)
    {
        _cash -= cash;
        _canvases.GameCanvas.CashPanel.transform.DORewind();
        _canvases.GameCanvas.CashPanel.transform.DOShakeRotation(0.5f, 10);

        SaveCash();
    }

    private void RefreshCash()
    {
        _canvases.GameCanvas.RefreshCash(_cash);
    }

    private void SaveCash()
    {
        PlayerPrefs.SetInt(Keys.PlayerPrefs.Cash.ToString(), _cash);
        RefreshCash();
        MMVibrationManager.Haptic(HapticTypes.LightImpact, false, true, this);
    }
}
