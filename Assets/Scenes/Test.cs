using DG.Tweening;
using MoreMountains.NiceVibrations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    
    private class TestClass
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _object;
        [SerializeField] private TMP_Text _cost;
        [SerializeField] private int _costValue;

        private bool _isPlayerEntry;
        private Vector3 _startCanvasScale;
        private Progress _progress;

        [JsonIgnore]
        public GameObject Object => _object;
        public int CostValue => _costValue;

        public TestClass(int cost, GameObject @object)
        {
            _costValue = cost;
            _object = @object;
        }
    }
    
    private List<List<TestClass>> _objects = new();
    private const string _fileName = "Save/Progress.json";
    private int cost = 10;

    public int Cost => cost;

    private void Start()
    {

    }

    private void Save()
    {
        var json = JsonConvert.SerializeObject(_objects, Formatting.Indented);

        File.WriteAllText($"{Application.dataPath}/{_fileName}", json);
    }
}
