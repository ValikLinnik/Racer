using System;
using UnityEngine;

[Serializable]
public class WheelView : OverrodeOperator
{
    [SerializeField]
    private WheelCollider _wheelCollider;

    [SerializeField]
    private Transform _modelTransform;

    public WheelCollider WheelColl
    {
        get
        {
            return _wheelCollider;
        }
        set
        {
            _wheelCollider = value;
        }
    }

    public Transform ModelTransform
    {
        get
        {
            return _modelTransform;
        }

        set
        {
            _modelTransform = value;
        }
    }

}


