using System;
using UnityEngine;
using Injection;

public class CarEngine : InjectorBase<CarEngine>, IDisposable
{
    #region SERIALIZE FIELDS

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField, Range(0, 100)]
    private float _maxSteerAngle = 45f;

    [SerializeField, Range(0, 30)]
    private float _timeToReach100 = 10f;

    [SerializeField, Range(0, 300)]
    public float _maxSpeed = 190;

    [SerializeField]
    private Transform _centerOfMass;

    [SerializeField]
    private WheelCollider _wheelFL;

    [SerializeField]
    private WheelCollider _wheelFR;

    [SerializeField]
    private WheelCollider _wheelBR;

    [SerializeField]
    private WheelCollider _wheelBL;

    [SerializeField]
    private AnimationCurve _torqueCurve;

    [SerializeField]
    private bool _isFourWheelDrive;

    #endregion

    #region PRIVATE FIELDS

    private float _currentSpeed;
    private float _wheelConstant;
    private float _torque;
    private float _timeDrivePressed = 1;
    private float _timeEventDriveWasPressed = -1;
    private EngineGear _currentGear = EngineGear.N;
    private bool _isBreaking;
    private float _maxTorque = 300;
    private float _brakingTorque = 500;

    #endregion

    #region PRIVATE PROPERTIES

    private float Torque
    {
        get
        {
            if (_torqueCurve.IsNull())
            {
                throw new NullReferenceException("CarEngine.Torque: TorqueCurve is null.");
            }

            var curveValue = _timeDrivePressed == -1 ? 0 : _torqueCurve.Evaluate(Mathf.Clamp01(_timeDrivePressed / _timeToReach100)) * _maxTorque;
            curveValue = _currentSpeed > _maxSpeed || _isBreaking ? 0 : curveValue;
            curveValue = _currentGear == EngineGear.R ? -curveValue : curveValue;
            return curveValue;
        }
    }

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if (_centerOfMass && _rigidbody) _rigidbody.centerOfMass = _centerOfMass.localPosition;
        if (_wheelFL) _wheelConstant = 2 * Mathf.PI * _wheelFL.radius * 60;
    }

    /// <summary>
    /// Sorry about this. I know, I need InputManager.
    /// </summary>
    private void LateUpdate()
    {
        _isBreaking = Input.GetKey(KeyCode.Space);
            
        var forward = Input.GetKeyDown(KeyCode.UpArrow);
        var backward = Input.GetKeyDown(KeyCode.DownArrow);

        if (forward || backward)
        {
            _timeDrivePressed = 0;
            _timeEventDriveWasPressed = Time.time;
            _currentGear = backward ? EngineGear.R : EngineGear.D;
            return;
        }

        forward = Input.GetKey(KeyCode.UpArrow);
        backward = Input.GetKey(KeyCode.DownArrow);

        if (forward || backward)
        {
            if (_timeEventDriveWasPressed == -1)
                _timeEventDriveWasPressed = Time.time;
            _timeDrivePressed = Time.time - _timeEventDriveWasPressed;
            _currentGear = backward ? EngineGear.R : EngineGear.D;
        }
        else
        {
            _timeDrivePressed = -1;
            _timeEventDriveWasPressed = -1;
        }
    }

    private void FixedUpdate()
    {
        CarMovementHandler();
    }

//    private void OnGUI()
//    {
//        GUI.Button(new Rect(10, 10, 100, 50), _currentSpeed.ToString("F"));
//        GUI.Button(new Rect(10, 60, 100, 50), _torque.ToString("F"));
//        GUI.Button(new Rect(10, 110, 100, 50), _currentGear.ToString());
//    }

    #endregion

    #region PRIVATE METHODS

    private void CarMovementHandler()
    {
        if(!_wheelFL || !_wheelFR || !_wheelBR || !_wheelBL) throw new NullReferenceException("All wheels colliders have to be attached.");

        var newSteer = Input.GetAxis("Horizontal");

        newSteer *= _maxSteerAngle;

        _wheelFL.steerAngle = newSteer;
        _wheelFR.steerAngle = newSteer;

        if (_isFourWheelDrive) 
        {
            _wheelBR.steerAngle = -newSteer;
            _wheelBL.steerAngle = -newSteer;
        }

        _currentSpeed = _wheelConstant * _wheelFL.rpm / 1000;
        _torque = Torque;

        if (_wheelFL.motorTorque != _torque) _wheelFL.motorTorque = _torque;
        if (_wheelFR.motorTorque != _torque) _wheelFR.motorTorque = _torque;

        if (!_isFourWheelDrive)
        {
            if (_wheelBL.motorTorque != _torque) _wheelBL.motorTorque = _torque;
            if (_wheelBR.motorTorque != _torque) _wheelBR.motorTorque = _torque;
        }

        var val = _isBreaking ? _brakingTorque : 0;
        if (_wheelFL.brakeTorque != val) _wheelFL.brakeTorque = val;
        if (_wheelFR.brakeTorque != val) _wheelFR.brakeTorque = val;
        if (_wheelBL.brakeTorque != val) _wheelBL.brakeTorque = val;
        if (_wheelBR.brakeTorque != val) _wheelBR.brakeTorque = val;
    }

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        if(_rigidbody) _rigidbody.velocity = Vector3.zero;

        if (_wheelFL) 
        {
            _wheelFL.brakeTorque = float.MaxValue;
            _wheelFL.motorTorque = 0;
        }

        if (_wheelFR) 
        {
            _wheelFR.brakeTorque = float.MaxValue;
            _wheelFR.motorTorque = 0;
        }

        if (_wheelBL) 
        {
            _wheelBL.brakeTorque = float.MaxValue;
            _wheelBL.motorTorque = 0;
        }

        if (_wheelBR) 
        {
            _wheelBR.brakeTorque = float.MaxValue;
            _wheelBR.motorTorque = 0;
        }

        _isBreaking = false;
        _currentGear = EngineGear.N;
    }

    #endregion
}

