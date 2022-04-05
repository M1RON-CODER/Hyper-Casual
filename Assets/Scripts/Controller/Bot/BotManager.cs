using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    [SerializeField] private GameObject _bot;
    [SerializeField] private Sprites _sprites;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<Resource> _waypoints = new List<Resource>();
    [SerializeField] private List<GameObject> _cashRegisters = new List<GameObject>();
    [Min(4)] [SerializeField] private int _maxCountBotOnOneWaypoint;

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
            bot.GetComponent<BotController>().Initialize(_waypoints, _cashRegisters, _sprites);
            bot.GetComponent<BotController>().SetWaypoints();
            
            _bots.Add(bot);
        }
        
    }
}
