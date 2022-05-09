using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointEmployee : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _employee;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private int _costValue;

    private bool _isHavePlayer;
    private Vector3 _startCanvasScale;
    private Progress _progress;

    public GameObject Employee => _employee;

    #region MonoBehaviour
    private void Start()
    {
        _startCanvasScale = _canvas.transform.localScale;
        _cost.text = _costValue.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _isHavePlayer = true;
            ChangeScaleInformationCanvas(_canvas.transform.localScale * 1.2f);
            StartCoroutine(BuyEmployee(player));
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        ChangeScaleInformationCanvas(_startCanvasScale);
        _isHavePlayer = false;
    }
    #endregion

    public void Initialize(Progress progress)
    {
        _progress = progress;
    }

    public void Active()
    {
        _employee.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ChangeScaleInformationCanvas(Vector3 scale)
    {
        _canvas.transform.DOScale(scale, 0.5f);
    }

    private IEnumerator BuyEmployee(Player player)
    {
        yield return new WaitForSeconds(1.2f);

        while (_costValue > 0)
        {
            if (!_isHavePlayer)
            {
                yield break;
            }

            if (player.CashData.Cash == 0)
            {
                Debug.Log("Не хватает денег");
                break;
            }
   
            int amountWithdrawal = UnityEngine.Random.Range(2, 40);
            int withdrawCash = (player.CashData.Cash >= amountWithdrawal) ? ((amountWithdrawal > _costValue) ? _costValue : amountWithdrawal) : player.CashData.Cash;
            player.CashData.WithdrawCash(withdrawCash);

            _costValue -= withdrawCash;
            _cost.text = _costValue.ToString();

            yield return new WaitForSeconds(0.15f);         
        }

        _progress.IncreaseProgress();
        Active();
    }
}
