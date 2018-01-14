using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WheelsController : MonoBehaviour , IDisposable
{
    #region SERIALIZE FIELDS

    [SerializeField, Range(0, 100)]
    private float _maxSteerAngle = 45f;

    [SerializeField]
    private WheelView _wheelFL;

    [SerializeField]
    private WheelView _wheelFR;

    [SerializeField]
    private WheelView _wheelBL;

    [SerializeField]
    private WheelView _wheelBR;

    #endregion	

    #region PRIVATE FIELDS

    private bool _isFourWheelDrive;

    public bool IsFourWheelDrive
    {
        get
        {
            return _isFourWheelDrive;
        }

        set
        {
            _isFourWheelDrive = value;
        }
    }

    #endregion

    public bool IsWheelsReady
    {
        get
        {
            return _wheelFL && _wheelFR && _wheelBR && _wheelBL && _wheelFL.WheelColl && _wheelFR.WheelColl && _wheelBR.WheelColl && _wheelBL.WheelColl;
        }
    }

    public float Radius
    {
        get
        {
            return _wheelFL.WheelColl ? _wheelFL.WheelColl.radius : 0;
        }
    }

    public float RPM
    {
        get 
        {
            return _wheelFL.WheelColl ? _wheelFL.WheelColl.rpm : 0;
        }
    }

    private void LateUpdate()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        if(_wheelFL) SetTransform(_wheelFL.ModelTransform, _wheelFL.WheelColl);
        if(_wheelFR) SetTransform(_wheelFR.ModelTransform, _wheelFR.WheelColl);
        if(_wheelBL) SetTransform(_wheelBL.ModelTransform, _wheelBL.WheelColl);
        if(_wheelBR) SetTransform(_wheelBR.ModelTransform, _wheelBR.WheelColl);
    }

    private void SetTransform(Transform transformIn, WheelCollider colliderIn)
    {
        if(!transformIn || !colliderIn) return;

        Vector3 position;
        Quaternion rotation;

        colliderIn.GetWorldPose(out position, out rotation);
        transformIn.position = position;
        transformIn.rotation = rotation;
    }

    public void CarMovementHandler(float val)
    {
        val *= _maxSteerAngle;

        _wheelFL.WheelColl.steerAngle = val;
        _wheelFR.WheelColl.steerAngle = val;

        if (_isFourWheelDrive) 
        {
            _wheelBR.WheelColl.steerAngle = -val;
            _wheelBL.WheelColl.steerAngle = -val;
        }
    }

    public void Torque(float torque)
    {
        _wheelFL.WheelColl.motorTorque = torque;
        _wheelFR.WheelColl.motorTorque = torque;

//        if (!_isFourWheelDrive)
//        {
//            _wheelBL.WheelColl.motorTorque = torque;
//            _wheelBR.WheelColl.motorTorque = torque;
//        }
    }

    public void Break(float val)
    {
        _wheelFL.WheelColl.brakeTorque = val;
        _wheelFR.WheelColl.brakeTorque = val;
        _wheelBL.WheelColl.brakeTorque = val;
        _wheelBR.WheelColl.brakeTorque = val;
    }

    #region IDisposable implementation

    public void Dispose()
    {
        Break(float.MaxValue);
        Torque(0);
    }

    #endregion
}
