using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cash : MonoBehaviour
{
    [SerializeField] private GameObject _cashPrefab;
    [SerializeField] private List<Transform> _cashPositions = new();
    [Min(3)] [SerializeField] private int _productCost = 3;

    private Dictionary<GameObject, bool> _cash = new();
    private Player _player;
    private int _cashAmount;
    private int _cashPosition;

    public int ProductCost => _productCost;

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

    public void AddCashOnCashRegister(Transform playerPosition, int cash)
    {
        var count = cash / _productCost;

        for (int i = 0; i < count; i++)
        {
            if (_cashPosition >= _cashPositions.Count)
            {
                _cashPosition = 0;
                _cashPositions.ForEach(x => x.position += Vector3.up * 0.25f);
            }

            GameObject cashObj = GetPooledObject();
            if(cashObj != null)
            {
                cashObj.transform.position = playerPosition.position;
                cashObj.SetActive(true);
                
                Debug.Log("Cash is Pooled");
            }
            else
            {
                cashObj = Instantiate(_cashPrefab, playerPosition.position, Quaternion.identity);
                cashObj.transform.SetParent(transform);
                _cash.Add(cashObj, false);

                Debug.Log("Cash is Instantiated");
            }

            cashObj.transform.DOMove(_cashPositions[_cashPosition].position, 0.3f).OnComplete(() =>
            {
                _cash[cashObj] = true;
            });
            
            _cashPosition++;
        }

        _cashAmount += cash;
/*        if (_player)
        {
            StartCoroutine(WithdrawCash(_player));
        }*/
    }

    public IEnumerator WithdrawCash(Player player)
    {
        float duration = 0.08f;
        foreach (var item in _cash.Reverse())
        {
            if ((_cashAmount == 0) || (!_player))
            {
                Debug.Log("Cash is Empty");
                yield break;
            }

            if (!item.Key.activeInHierarchy || !item.Value)
            {
                Debug.Log("Cash is not active");
                continue;
            }

            item.Key.transform.DOMove(new Vector3(player.transform.position.x, player.transform.position.y / 2, player.transform.position.z), duration).OnComplete(() =>
            {
                item.Key.SetActive(false);
            });

            player.CashData.AddCash(_productCost);
            _cashAmount -= _productCost;
            _cashPosition--;

            if (_cashPosition + 1 % _cashPositions.Count == 0)
            {
                _cashPositions.ForEach(x => x.position -= Vector3.up * 0.25f);
            }

            Debug.Log("Cash is withdrawn");
            yield return new WaitForSeconds(duration);
        }
/*            
        for (int i = _cash.Count - 1; i >= 0; i--)
        {
            if((_cashAmount == 0) || (!_player))
            {
                Debug.Log("Cash is Empty");
                yield break;
            }

            if (!_cash[])
            {
                Debug.Log("Cash is not active");
                continue;
            }
            
            _cash[i].transform.DOMove(new Vector3(player.transform.position.x, player.transform.position.y / 2, player.transform.position.z), duration).OnComplete(() =>
            {
                _cash[i].SetActive(false);
            });

            player.CashData.AddCash(_productCost);
            _cashAmount -= _productCost;
            _cashPosition--;

            if(_cashPosition + 1 % _cashPositions.Count == 0)
            {
                _cashPositions.ForEach(x => x.position -= Vector3.up * 0.25f);
            }

            Debug.Log("Cash is withdrawn");
            yield return new WaitForSeconds(duration);
        }*/

        _cashPositions.ForEach(cashPosition => cashPosition.localPosition = new Vector3(cashPosition.localPosition.x, 0, cashPosition.localPosition.z));
        _cashAmount = 0;
        _cashPosition = 0;
    }

    private GameObject GetPooledObject()
    {
        foreach(GameObject poolObject in _cash.Keys)
        {
            if (!poolObject.activeInHierarchy)
            {
                return poolObject;
            }
        }

        return null;
    }
}