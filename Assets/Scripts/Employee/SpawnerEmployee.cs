using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnerEmployee : MonoBehaviour
{
    [SerializeField] private GameObject _employee;
    [SerializeField] private TMP_Text _cost;

    private bool _isHavePlayer;
    private Progress _progress;

    public GameObject Employee => _employee;

    #region MonoBehaviour
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
        
        int count = Convert.ToInt32(_cost.text);
        while(count > 0)
        {
            var i = UnityEngine.Random.Range(5, 25);

            if (player.CashData.Cash < i)
            {
                break;
            }

            _cost.text = i < 0 ? "0" : i.ToString();
            player.CashData.WithdrawCash((i > count) ? count : i);
            count -= i;

            yield return new WaitForSeconds(0.15f);
        }

        _progress.IncreaseProgress();
        Active();
    }
}
