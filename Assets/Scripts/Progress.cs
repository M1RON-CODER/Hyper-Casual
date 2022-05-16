using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class Progress : MonoBehaviour
{
    [Serializable]
    public class Object
    {
        public List<EmployeePoint> employees = new ();
    }

    [SerializeField] private List<Object> _objects = new ();

    private int _progressIndex;
    private const string _fileName = "Progress.json";
    
    #region MonoBehaviour
    private void Awake()
    {
        _progressIndex = PlayerPrefs.GetInt(Keys.Progress.ProgressIndex.ToString(), 0);

        FirstInitialize();
        ReadFromSave();
        ActivateObjects();
    }
    #endregion

    public void IncreaseProgress()
    {
        var progress = _objects[_progressIndex].employees.Count(x => x.Cost > 0);
        if (progress == 0)
        {
            _progressIndex++;
            PlayerPrefs.SetInt(Keys.Progress.ProgressIndex.ToString(), _progressIndex);
        }
    }
    
    public void ShowNextObject()
    {
        if (_progressIndex >= _objects.Count)
        {
            return;
        }

        for (int i = 0; i < _objects[_progressIndex].employees.Count; i++)
        {
            if (_objects[_progressIndex].employees[i].Cost > 0)
            {
                _objects[_progressIndex].employees[i].EmployeePointBase.Show();
            }
        }
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(_objects, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        File.WriteAllText($"{Application.dataPath}/Save/{_fileName}", json);
    }

    private void ActivateObjects()
    {
        for (int i = 0; i < _progressIndex + 1; i++)
        {
            for (int j = 0; j < _objects[i].employees.Count; j++)
            {
                if (_objects[i].employees[j].Cost <= 0)
                {
                    _objects[i].employees[j].EmployeePointBase.Activate();
                }
            }
        }

        ShowNextObject();
    }

    private void FirstInitialize()
    {
        foreach (Object @object in _objects)
        {
            foreach (EmployeePoint employee in @object.employees)
            {
                employee.Initialize(this);
            }
        }
    }

    
    private void ReadFromSave()
    {
        var json = JsonConvert.DeserializeObject<List<Object>>(File.ReadAllText($"{Application.dataPath}/Save/{_fileName}"));
        for (int i = 0; i < json.Count; i++)
        {
            for (int j = 0; j < json[i].employees.Count; j++)
            {
                _objects[i].employees[j].Cost = json[i].employees[j].Cost;
            }
        }
    }
}

