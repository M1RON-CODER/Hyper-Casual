using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class Cash : MonoBehaviour
{
    [SerializeField] private GameObject _cashPrefab;
    [SerializeField] private List<Transform> _cashPositions = new();
    [Min(3)] [SerializeField] private int _productCost = 3;

    [JsonProperty("Cost Amount")] private int _cashAmount;
    private List<GameObject> _cash = new();
    private Player _player;
    private int _cashPosition;
    private int _maxMoneyHeight = 8;

    public int ProductCost => _productCost;
    public int CashAmount => _cashAmount;

    #region MonoBehaviour
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _player = player;
            StartCoroutine(WithdrawCash(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _player = null;
    }
    #endregion

    public void Initialize(int cash)
    {
        AddCashOnCashRegister(null, cash);
    }

    public void AddCashOnCashRegister(Transform playerPosition, int cash)
    {
        var count = cash / _productCost;

        for (int i = 0; i < count; i++)
        {
            if (_cashPosition >= _cashPositions.Count)
            {
                if ((_cashAmount / _productCost / _cashPositions.Count) >= _maxMoneyHeight)
                {
                    continue;
                }
                
                _cashPosition = 0;
                _cashPositions.ForEach(x => x.position += Vector3.up * 0.25f);
            }

            GameObject cashObj = GetPooledObject();
            if(cashObj != null)
            {
                cashObj.transform.position = playerPosition.position;
                cashObj.SetActive(true);
            }
            else
            {
                cashObj = Instantiate(_cashPrefab, playerPosition.position, Quaternion.identity);
                cashObj.transform.SetParent(transform);
                _cash.Add(cashObj);
            }

            if (!_player)
            {
                cashObj.transform.DOMove(_cashPositions[_cashPosition].position, 0.3f);
            }
            else
            {
                WithdrawCash(_player, cashObj);
            }
            
            _cashPosition++;
        }

        _cashAmount += cash;
    }

    public IEnumerator WithdrawCash(Player player)
    {
        float duration = 0.08f;
        for (int i = _cash.Count - 1; i >= 0; i--)
        {
            if ((_cashAmount == 0) || (!_player))
            {
                yield break;
            }

            if (!_cash[i].activeInHierarchy)
            {
                continue;
            }

            _cash[i].transform.DOMove(new Vector3(player.transform.position.x, player.transform.position.y / 2, player.transform.position.z), duration).OnComplete(() =>
            {
                _cash[i].SetActive(false);
            });

            if (_cashPosition + 1 % _cashPositions.Count == 0)
            {
                _cashPositions.ForEach(x => x.position -= Vector3.up * 0.25f);
            }

            yield return new WaitForSeconds(duration);
        }

        _cashPositions.ForEach(cashPosition => cashPosition.localPosition = new Vector3(cashPosition.localPosition.x, 0, cashPosition.localPosition.z));
        _cashAmount = 0;
        _cashPosition = 0;
    }

    public void WithdrawCash(Player player, GameObject cash)
    {
        cash.transform.DOMove(_player.transform.position + new Vector3(0, 0.7f, 0), 0.3f).OnComplete(() =>
        {
            _cashAmount -= _productCost;
            _cashPosition--;
            
            player.CashData.AddCash(_productCost);
            cash.SetActive(false);

        });
    }

    private GameObject GetPooledObject()
    {
        foreach(GameObject poolObject in _cash)
        {
            if (!poolObject.activeInHierarchy)
            {
                return poolObject;
            }
        }

        return null;
    }
}