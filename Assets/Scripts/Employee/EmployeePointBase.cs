using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class EmployeePointBase : MonoBehaviour, IProgress
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _object;
    [SerializeField] private TMP_Text _cost;

    private Progress _progress;
    private EmployeePoint _employeePoint;
    private bool _isPlayerEntry;
    private Vector3 _startCanvasScale;

    public GameObject Object => _object;


    #region MonoBehaviour
    private void Start()
    {
        _startCanvasScale = _canvas.transform.localScale;
        _cost.text = _employeePoint.Cost.ToString();
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

    public void Initialize(Progress progress, EmployeePoint employeePoint)
    {
        _progress = progress;
        _employeePoint = employeePoint;
    }

    public void Activate()
    {
        _object.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void ChangeScaleInformationCanvas(Vector3 scale)
    {
        _canvas.transform.DOScale(scale, 0.5f);
    }

    private IEnumerator BuyObject(Player player)
    {
        yield return new WaitForSeconds(1.2f);

        while (_employeePoint.Cost > 0)
        {
            if (!_isPlayerEntry)
            {
                _progress.Save();
                yield break;
            }

            if (player.CashData.Cash == 0)
            {
                Debug.Log("No money");
                break;
            }

            int amountWithdrawal = UnityEngine.Random.Range(2, 40);
            int withdrawCash = (player.CashData.Cash >= amountWithdrawal) ? ((amountWithdrawal > _employeePoint.Cost) ? _employeePoint.Cost : amountWithdrawal) : player.CashData.Cash;
            player.CashData.WithdrawCash(withdrawCash);

            _employeePoint.Withdraw(withdrawCash);
            _cost.text = _employeePoint.Cost.ToString();

            yield return new WaitForSeconds(0.15f);
        }

        _progress.IncreaseProgress();
        _progress.Save();
        Activate();
        _progress.ShowNextObject();
    }
}
