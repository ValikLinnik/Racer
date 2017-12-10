using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarWheel : MonoBehaviour 
{	
    [SerializeField]
    private WheelCollider _targetWheel;

    private Vector3 _position;
    private Quaternion _rotation;

    private void LateUpdate()
    {
        if(!_targetWheel) throw new NullReferenceException("Wheel collider is null.");

        _targetWheel.GetWorldPose(out _position, out _rotation);
        transform.position = _position;
        transform.rotation = _rotation;

    }
}
