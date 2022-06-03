using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPoints = new ();
    [SerializeField] private List<Rack> _racks = new ();
    [SerializeField] private List<CashRegister> _cashRegisters = new ();
    [SerializeField] private List<Transform> _exitPoints;
    [SerializeField] private GameObject _buyerPrefab;
    [SerializeField] private Skin _skin;
    [SerializeField] [Min(0)] private int _maxCountAIOnOneRack;

    private List<GameObject> _byers = new ();
    private int[] _racksIndexes;
    private int _maxCountByers;
    private int _numberAI;
    private bool _isPaused;

    #region MonoBehavior

    private void Start()
    {
        _cashRegisters = JSON.ReadFileCashRegister(_cashRegisters);
        _maxCountByers = _racks.Count * _maxCountAIOnOneRack;

        _racksIndexes = new int[_racks.Count];
        for (int i = 0; i < _racks.Count; i++)
        {
            _racksIndexes[i] = 0;
        }

        StartCoroutine(CreateBot());
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        _isPaused = !hasFocus;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        _isPaused = pauseStatus;
        if (_isPaused)
        {
            JSON.SaveCashRegister(_cashRegisters);
        }
    }
    #endregion

    public void AddRack(Rack resource)
    {
        _racks.Add(resource);
    }

    public void DestroyAI(Buyer buyer)
    {
        Destroy(buyer.gameObject);
    }

    public void RemoveAIFromQueue(Buyer buyer)
    {
        _byers.Remove(buyer.gameObject);
    }

    private IEnumerator CreateBot()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 3));

            if (_byers.Count == _maxCountByers)
            {
                continue;
            }

            List<Rack> racks = GetRacks();
            CashRegister cashRegister = GetNearestCashRegister(racks.Last());

            GameObject buyerObj = Instantiate(_buyerPrefab, _spawnPoints[Random.Range(0, _spawnPoints.Count)].transform.position, Quaternion.identity);
            buyerObj.name = $"Byer_{++_numberAI}";

            if (buyerObj.TryGetComponent(out Buyer buyer))
            {
                buyer.Skin.material = _skin.Skins[Random.Range(0, _skin.Skins.Count)];
                buyer.Initialize(this, cashRegister, _exitPoints[Random.Range(0, _exitPoints.Count)]);
                buyer.SetTargets(racks);
            }

            _byers.Add(buyerObj);
        }
    }

    private List<Rack> GetRacks()
    {
        List<Rack> allRacks = _racks.ToList();
        List<Rack> racks = new ();

        int count = allRacks.Count >= 4 ? Random.Range(1, 4) : Random.Range(1, allRacks.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, allRacks.Count);
            if(_racksIndexes[index] != _maxCountAIOnOneRack)
            {
                racks.Add(allRacks[index]);
                allRacks.RemoveAt(index);
                _racksIndexes[index]++;
            }
            else
            {
                for (int j = 0; j < allRacks.Count; j++)
                {
                    if (_racksIndexes[j] != _maxCountAIOnOneRack)
                    {
                        racks.Add(allRacks[j]);
                        allRacks.RemoveAt(j);
                        _racksIndexes[j]++;
                        break;
                    }
                }

                if (racks.Count == 0)
                {
                    racks.Add(_racks[Random.Range(0, _racks.Count)]);
                    return racks;
                }
            }
        }

        return racks;
    }

    private CashRegister GetNearestCashRegister(Rack lastObject)
    {
        Dictionary<CashRegister, float> cashRegisters = new Dictionary<CashRegister, float>();

        foreach (CashRegister cashRegister in _cashRegisters)
        {
            cashRegisters.Add(cashRegister, Vector3.Distance(cashRegister.transform.position, transform.position));
        }

        return cashRegisters.Where(x => x.Value == cashRegisters.Values.Min()).First().Key;
    }
}
