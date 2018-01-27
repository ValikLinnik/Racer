using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WheelsController : MonoBehaviour , IDisposable
{
    #region SERIALIZE FIELDS

    [SerializeField, Range(0, 10)]
    private float _maxSteerAngle = 2f;

    [SerializeField]
    private WheelView _wheelFL;

    [SerializeField]
    private WheelView _wheelFR;

    [SerializeField]
    private WheelView _wheelBL;

    [SerializeField]
    private WheelView _wheelBR;

    [SerializeField, Range(0.1f, 20f), Tooltip("Natural frequency of the suspension springs. Describes bounciness of the suspension.")]
    private float _naturalFrequency = 10;

    [SerializeField, Range(0f, 3f), Tooltip("Damping ratio of the suspension springs. Describes how fast the spring returns back after a bounce. ")]
    private float _dampingRatio = 0.8f;

    [SerializeField, Range(-1f, 1f), Tooltip("The distance along the Y axis the suspension forces application point is offset below the center of mass")]
    private float _forceShift = 0.03f;

    [SerializeField, Tooltip("Adjust the length of the suspension springs according to the natural frequency and damping ratio. When off, can cause unrealistic suspension bounce.")]
    private bool _setSuspensionDistance = true;

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

    #region PUBLIC PROPERTIES

    public Rigidbody CarBody
    {
        private get;
        set;
    }

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

    #endregion

    #region UNITY EVENTS

    private void Update()
    {
        UpdateWheelsForces();
    }

    private void LateUpdate()
    {
        UpdateView();
    }

    #endregion

    #region PUBLIC METHODS

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

        if (!_isFourWheelDrive)
        {
            _wheelBL.WheelColl.motorTorque = torque;
            _wheelBR.WheelColl.motorTorque = torque;
        }
    }

    public void Break(float val)
    {
        _wheelFL.WheelColl.brakeTorque = val;
        _wheelFR.WheelColl.brakeTorque = val;
        _wheelBL.WheelColl.brakeTorque = val;
        _wheelBR.WheelColl.brakeTorque = val;
    }

    #endregion

    #region PRIVATE METHODS

    private void UpdateWheelsForces()
    {
        UpdateForces(_wheelFL);
        UpdateForces(_wheelFR);
        UpdateForces(_wheelBL);
        UpdateForces(_wheelBR);
    }

    private void UpdateForces(WheelView wheelView)
    {
        if(!wheelView || wheelView.WheelColl) return;

        JointSpring spring = wheelView.WheelColl.suspensionSpring;

        float sqrtWcSprungMass = Mathf.Sqrt (wheelView.WheelColl.sprungMass);
        spring.spring = sqrtWcSprungMass * _naturalFrequency * sqrtWcSprungMass * _naturalFrequency;
        spring.damper = 2f * _dampingRatio * Mathf.Sqrt(spring.spring * wheelView.WheelColl.sprungMass);

        wheelView.WheelColl.suspensionSpring = spring;

        Vector3 wheelRelativeBody = CarBody.transform.InverseTransformPoint(wheelView.WheelColl.transform.position);
        float distance = CarBody.centerOfMass.y - wheelRelativeBody.y + wheelView.WheelColl.radius;

        wheelView.WheelColl.forceAppPointDistance = distance - _forceShift;

        if (spring.targetPosition > 0 && _setSuspensionDistance)
            wheelView.WheelColl.suspensionDistance = wheelView.WheelColl.sprungMass * Physics.gravity.magnitude / (spring.targetPosition * spring.spring);
    }

    private void UpdateView()
    {
        if (_wheelFL) SetTransform(_wheelFL.ModelTransform, _wheelFL.WheelColl);
        if (_wheelFR) SetTransform(_wheelFR.ModelTransform, _wheelFR.WheelColl);
        if (_wheelBL) SetTransform(_wheelBL.ModelTransform, _wheelBL.WheelColl);
        if (_wheelBR) SetTransform(_wheelBR.ModelTransform, _wheelBR.WheelColl);
    }

    private void SetTransform(Transform transformIn, WheelCollider colliderIn)
    {
        if (!transformIn || !colliderIn) return;

        Vector3 position;
        Quaternion rotation;

        colliderIn.GetWorldPose(out position, out rotation);
        transformIn.position = position;
        transformIn.rotation = rotation;
    }

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        Break(float.MaxValue);
        Torque(0);
    }

    #endregion
}
