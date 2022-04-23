using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnerEmployee : MonoBehaviour
{
    [SerializeField] private GameObject _employee;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private int _costValue;

    private bool _isHavePlayer;
    private Progress _progress;

    public GameObject Employee => _employee;

    #region MonoBehaviour
    private void Start()
    {
        _cost.text = _costValue.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            _isHavePlayer = true;
            StartCoroutine(BuyEmployee(player));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        _isHavePlayer = false;
    }
    #endregion

    public void Initialize(Progress progress)
    {
        _progress = progress;
        Debug.Log(_progress);
    }
    
    public void Active()
    {
        _employee.SetActive(true);
        gameObject.SetActive(false);
    }
    
    private IEnumerator BuyEmployee(Player player)
    {
        yield return new WaitForSeconds(1.5f);

        if (!_isHavePlayer)
        {
            yield break;
        }

        while(_costValue > 0)
        {
            var i = UnityEngine.Random.Range(5, 25);

            if (player.CashData.Cash == 0)
            {
                Debug.Log("Не хватает денег");
                break;
            }
            int withdrawCash = (player.CashData.Cash >= i) ? ((i > _costValue) ? _costValue : i) : player.CashData.Cash;
            Debug.Log(withdrawCash);
            player.CashData.WithdrawCash(withdrawCash);

            //Debug.Log($"i: {i} _costValue: {_costValue}");
            _costValue -= withdrawCash;
            _cost.text = _costValue.ToString();

            yield return new WaitForSeconds(0.15f);         
        }
        //_progress.IncreaseProgress();
        // Active();
    }
}
