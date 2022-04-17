using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    [SerializeField] private GameObject _bot;
    [SerializeField] private Sprites _sprites;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<GameObject> _waypoints = new List<GameObject>();
    [SerializeField] private List<CashRegister> _cashRegisters = new List<CashRegister>();
    [SerializeField] private Transform _exitPoint;
    [Min(1)] [SerializeField] private int _maxCountBotOnOneWaypoint;

    private List<GameObject> _bots = new List<GameObject>();
    private int _maxCountBots;
    private int _numberBot;

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
                continue;
            }

            List<GameObject> waypoints = GetWaypoints();
            CashRegister cashRegister = GetNearestCashRegister(waypoints.Last());
            
            GameObject bot = Instantiate(_bot, _spawnPoints[Random.Range(0, _spawnPoints.Count)].transform.position, Quaternion.identity);
            bot.name = $"Bot_{++_numberBot}";
            bot.GetComponent<BotController>().Initialize(this, cashRegister, _exitPoint);
            bot.GetComponent<BotController>().SetTargets(waypoints);
            
            _bots.Add(bot);
        }  
    }

    public void DestroyBot(BotController bot)
    {
        Destroy(bot.gameObject);
    }

    public void RemoveBotFromQueue(BotController bot)
    {
        _bots.Remove(bot.gameObject);
    }

    private List<GameObject> GetWaypoints()
    {
        List<GameObject> allWaypoints = _waypoints.ToList();
        List<GameObject> waypoints = new List<GameObject>(); 
        for (int i = 0; i < (allWaypoints.Count >= 4 ? Random.Range(1, 4) : Random.Range(1, allWaypoints.Count)); i++)
        {
            int index = Random.Range(0, allWaypoints.Count);
            waypoints.Add(allWaypoints[index]);
            allWaypoints.RemoveAt(index);
        }

        return waypoints;
    }
    
    private CashRegister GetNearestCashRegister(GameObject lastObject)
    {
        Dictionary<CashRegister, float> cashRegisters = new Dictionary<CashRegister, float>();

        foreach (CashRegister cashRegister in _cashRegisters)
        {
            cashRegisters.Add(cashRegister, Vector3.Distance(cashRegister.transform.position, transform.position));
        }

        return cashRegisters.Where(x => x.Value == cashRegisters.Values.Min()).First().Key;
    }
}
