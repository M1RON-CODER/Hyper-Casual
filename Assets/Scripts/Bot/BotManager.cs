using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    [SerializeField] private GameObject _bot;
    [SerializeField] private Sprites _sprites;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<GameObject> _waypoints = new List<GameObject>();
    [SerializeField] private List<CashRegister> _cashRegisters = new List<CashRegister>();
    [SerializeField] private Transform _exit;
    [Min(1)] [SerializeField] private int _maxCountBotOnOneWaypoint;

    private List<GameObject> _bots = new List<GameObject>();
    private int _maxCountBots;

    #region MonoBehavior
    
    private void Start()
    {
        _maxCountBots = _waypoints.Count * _maxCountBotOnOneWaypoint;
        StartCoroutine(CreateBot());
    }
    #endregion

    private IEnumerator CreateBot()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(1, 3));

            if(_bots.Count == _maxCountBots)
            {
                yield break;
            }
            
            GameObject bot = Instantiate(_bot, _spawnPoints[Random.Range(0, _spawnPoints.Count)].transform.position, Quaternion.identity);    
            bot.GetComponent<BotController>().Initialize(_cashRegisters, _sprites, _exit);
            bot.GetComponent<BotController>().SetWaypoints(_waypoints);
            
            _bots.Add(bot);
        }  
    }
}
