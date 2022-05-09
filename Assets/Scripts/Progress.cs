using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Progress : MonoBehaviour
{
    [SerializeField] private List<List<PointEmployee>> _objects;
    private int _progressIndex;

    public int ProgressIndex => _progressIndex;
    
    #region MonoBehaviour
    private void Awake()
    {
        _progressIndex = PlayerPrefs.GetInt(Keys.PlayerPrefs.ProgressIndex.ToString(), 0);

        ActiveOpenObject();
    }
    #endregion

    public void IncreaseProgress()
    {
        _progressIndex++;
        PlayerPrefs.SetInt(Keys.PlayerPrefs.ProgressIndex.ToString(), _progressIndex);
    }

    private void ActiveOpenObject()
    {
        for (int i = 0; i < _progressIndex; i++)
        {
            _objects[i].Active();
        }

        ShowNextObject();
    }

    private void ShowNextObject()
    {
        if (_progressIndex < _objects.Count)
        {
            _objects[_progressIndex].Initialize(this);
            _objects[_progressIndex].gameObject.SetActive(true);
        }
    }
}

