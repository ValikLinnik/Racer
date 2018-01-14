using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointComponent : MonoBehaviour 
{
    [SerializeField, Range(.1f, 1f)]
    private float _rotationSpeed = 1f;

    private Transform _target;
    public Transform Target
    {
        get
        {
            return _target;
        }

        set
        {
            _target = value;
        }
    }

    private Quaternion _quaternion;

    private void LateUpdate()
    {
        if(!_target) 
        {
            transform.rotation = Quaternion.identity;
            return;
        }
        SetTargetQuaternion();
        transform.rotation = Quaternion.Lerp(transform.rotation, _quaternion, Time.deltaTime * _rotationSpeed);
    }

    private void SetTargetQuaternion()
    {
        if(!_target) 
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        var temp = Quaternion.LookRotation(_target.position - transform.position);
        _quaternion = Quaternion.Euler(0, temp.eulerAngles.y, 0);
    }
}
