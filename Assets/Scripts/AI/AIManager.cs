using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private GameObject _AIPrefab;
    [SerializeField] private Sprites _sprites;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<GameObject> _waypoints = new List<GameObject>();
    [SerializeField] private List<CashRegister> _cashRegisters = new List<CashRegister>();
    [SerializeField] private Transform _exitPoint;
    [Min(0)] [SerializeField] private int _maxCountAIOnOneWaypoint;

    private List<GameObject> _AI = new List<GameObject>();
    private int _maxCountAI;
    private int _numberAI;

    #region MonoBehavior

    private void Start()
    {
        _maxCountAI = _waypoints.Count * _maxCountAIOnOneWaypoint;
        StartCoroutine(CreateBot());
    }
    #endregion

    private IEnumerator CreateBot()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(1, 3));

            if(_AI.Count == _maxCountAI)
            {
                continue;
            }

            List<GameObject> waypoints = GetWaypoints();
            CashRegister cashRegister = GetNearestCashRegister(waypoints.Last());
            
            GameObject AI = Instantiate(_AIPrefab, _spawnPoints[Random.Range(0, _spawnPoints.Count)].transform.position, Quaternion.identity);
            AI.name = $"Bot_{++_numberAI}";

            if(AI.TryGetComponent(out AIController AIController))
            {
                AIController.Initialize(this, cashRegister, _exitPoint);
                AIController.SetTargets(waypoints);
            }
            
            _AI.Add(AI);
        }  
    }

    public void DestroyAI(AIController AI)
    {
        Destroy(AI.gameObject);
    }

    public void RemoveAIFromQueue(AIController AI)
    {
        _AI.Remove(AI.gameObject);
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
