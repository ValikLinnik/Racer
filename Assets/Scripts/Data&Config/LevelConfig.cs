using System;
using UnityEngine;

[Serializable]
public class LevelConfig
{
    [SerializeField]
    private float _timeToReach3Stars;
    [SerializeField]
    private float _timeToReach2Stars;
    [SerializeField]
    private float _timeToReach1Stars;

    public float TimeToReach3Stars
    {
        get
        {
            return _timeToReach3Stars;   
        }
    }

    public float TimeToReach2Stars
    {
        get
        {
            return _timeToReach2Stars;   
        }
    }

    public float TimeToReach1Stars
    {
        get
        {
            return _timeToReach1Stars;   
        }
    }
}
