using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Building : MonoBehaviour
{
/*    private class Params
    {
        private System.Action _action;
        private string _key;

        public Params(System.Action action, string key)
        {
            _action = action;
            _key = key;
        }

        public System.Action Action => _action;
        public string Key => _key;
    }

    private enum Method
    {
        LevelUp,
        GetCash,
    }

    [SerializeField] private Method _method;
    [SerializeField] private List<int> _prices = new List<int>();
    [SerializeField] private TMP_Text _price;

    private Dictionary<Method, Params> _methodLookup;
    private Collider _collider;
    private int _level;

    private void Awake()
    {
        _methodLookup = new Dictionary<Method, Params>()
        {
            { Method.LevelUp, new Params(LevelUp, Keys.PlayerLevel) },
            { Method.GetCash, new Params(GetCash, Keys.GetCash) },
        };

        _level = PlayerPrefs.GetInt(_methodLookup[_method].Key);
        _price.text = (_prices.Count - 1 == _level) ? "MAX" : $"{_prices[_level]} ({_level + 1}↑)";
    }

    private void LevelUp()
    {
        if(_level == _prices.Count - 1|| _prices.Count == 0)
        {
            _price.text = "MAX";
            return;
        }

        if (_collider.GetComponent<PlayerController>())
        {
            PlayerController player = _collider.GetComponent<PlayerController>();
            if (player.CashData.Cash >= _prices[_level])
            {
                player.CashData.WithdrawCash(_prices[_level]);
                _level++;
                _price.text = _prices[_level].ToString();

                PlayerPrefs.SetInt(Keys.PlayerLevel, _level);

                player.IncreaseMaxCountOnHands();
            }
        }
    }

    private void GetCash()
    {
        if (_collider.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            PlayerController player = playerController;
            player.DepositCash(100);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _collider = other;
        _methodLookup[_method].Action?.Invoke();
    }*/
}
