using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cash : MonoBehaviour
{
    [SerializeField] private GameObject _cashPrefab;
    [SerializeField] private List<Transform> _cashPositions = new();
    [Min(3)] [SerializeField] private int _productCost = 3;

    private List<GameObject> _cash = new();
    private List<Transform> _startPostion = new();
    private int _cashAmount;
    private int _cashPosition;

    public int ProductCost => _productCost;

    #region MonoBehaviour
    private void Start()
    {
        _startPostion = _cashPositions;
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
                for (int j = 0; j < 116; j += 12)
                {
                    if (cash <= j + 12)
                    {
                        var positionLevel = j / 12;
                        foreach (Transform position in _cashPositions)
                        {
                            position.position += Vector3.up * ((positionLevel == 0) ? 1 : positionLevel) * 0.25f;
                        }

                        break;
                    }
                }
            }

            GameObject cashObj = GetPooledObject();
            if(cashObj != null)
            {
                cashObj.transform.position = playerPosition.position;
                cashObj.transform.DOMove(_cashPositions[_cashPosition].position, 0.3f);
                cashObj.SetActive(true);
            }
            else
            {
                cashObj = Instantiate(_cashPrefab, playerPosition.position, Quaternion.identity);
                cashObj.transform.DOMove(_cashPositions[_cashPosition].position, 0.3f);
                _cash.Add(cashObj);
            }

            _cashPosition++;
        }

        _cashAmount += cash;
    }

    public void WithdrawCash(Player player)
    {
        if(_cashAmount == 0)
        {
            return;
        }

        foreach (GameObject cash in _cash)
        {
            cash.transform.DOMove(player.transform.position, 0.25f).OnComplete(() =>
            {
                cash.SetActive(false);
            });
        }


        player.CashData.AddCash(_cashAmount);
        _cashPositions = _startPostion;
        _cashAmount = 0;
        _cashPosition = 0;
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
