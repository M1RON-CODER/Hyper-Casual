using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointProgress : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _object;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private int _costValue;

    private bool _isPlayerEntry;
    private Vector3 _startCanvasScale;
    private Progress _progress;

    public GameObject Object => _object;

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
            _isPlayerEntry = true;
            ChangeScaleInformationCanvas(_canvas.transform.localScale * 1.2f);
            StartCoroutine(BuyObject(player));
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        ChangeScaleInformationCanvas(_startCanvasScale);
        _isPlayerEntry = false;
    }
    #endregion

    public void Initialize(Progress progress)
    {
        _progress = progress;
    }

    public void Active()
    {
        _object.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ChangeScaleInformationCanvas(Vector3 scale)
    {
        _canvas.transform.DOScale(scale, 0.5f);
    }

    private IEnumerator BuyObject(Player player)
    {
        yield return new WaitForSeconds(1.2f);

        while (_costValue > 0)
        {
            if (!_isPlayerEntry)
            {
                yield break;
            }

            if (player.CashData.Cash == 0)
            {
                Debug.Log("Не хватает денег");
                break;
            }
   
            int amountWithdrawal = Random.Range(2, 40);
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
