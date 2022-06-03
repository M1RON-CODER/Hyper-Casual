using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using TMPro;
using UnityEngine;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class ProgressPoint : MonoBehaviour, IProgress 
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] [Min(0)] [JsonProperty("Cost")] private int _costPrice;

    private Progress _progress;
    private bool _isPlayerEntry;
    private Vector3 _startCanvasScale;

    public int Cost => _costPrice;

    #region MonoBehaviour
    private void Start()
    {
        _startCanvasScale = _canvas.transform.localScale;
        _cost.text = _costPrice.ToString();
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

    public virtual void Activate()
    {
        gameObject.SetActive(false);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetPrice(int price)
    {
        _costPrice = price;
    }

    private void ChangeScaleInformationCanvas(Vector3 scale)
    {
        _canvas.transform.DOScale(scale, 0.5f);
    }

    private IEnumerator BuyObject(Player player)
    {
        yield return new WaitForSeconds(1.2f);

        int amountWithdrawal = 2;
        while (_costPrice > 0)
        {
            if (!_isPlayerEntry)
            {
                JSON.SaveProgress(_progress.Objects);
                yield break;
            }

            if (player.CashData.Cash == 0)
            {
                Debug.Log("No money");
                break;
            }
            
            int withdrawCash = (player.CashData.Cash >= amountWithdrawal) ? ((amountWithdrawal > _costPrice) ? _costPrice : amountWithdrawal) : player.CashData.Cash;
            player.CashData.WithdrawCash(withdrawCash);

            _costPrice -= withdrawCash;
            _cost.text = _costPrice.ToString();
            amountWithdrawal += 2;

            yield return new WaitForSeconds(0.15f);
        }

        _progress.IncreaseProgress();
        JSON.SaveProgress(_progress.Objects);
        Activate();
        _progress.ShowNextObject();
    }
}
