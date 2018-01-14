using System;
using UnityEngine;
using Injection;

public class CarEngine : MonoBehaviour, IDisposable
{
    #region SERIALIZE FIELDS

    [SerializeField]
    private WheelsController _wheelsController;

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField, Range(0, 30)]
    private float _timeToReach100 = 10f;

    [SerializeField, Range(0, 500)]
    public float _maxSpeed = 190;

    [SerializeField, Range(0, 1000), Tooltip("Max torque")]
    private float _maxTorque = 300;

    [SerializeField]
    private Transform _centerOfMass;

    [SerializeField]
    private AnimationCurve _torqueCurve;

    [SerializeField]
    private AudioClip _engineDefault;

    [SerializeField]
    private AudioClip _onSpeedUp;

    #endregion

    #region PUBLIC PROPERTIES

    public EngineGear EngineGear
    {
        get
        {
            return _currentGear;
        }
        set
        {
            _currentGear = value;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private float _currentSpeed;
    private float _wheelConstant;
    private float _torque;
    private float _timeDrivePressed = -1;
    private float _timeEventDriveWasPressed = -1;
    private EngineGear _currentGear = EngineGear.N;
    private bool _isBreaking;

    public bool IsBreaking
    {
        get
        {
            return _isBreaking;
        }

        set
        {
            _isBreaking = value;
        }
    }

    private AudioSource _engineSounSource;

    private AudioSource _engineSounSourceProp
    {
        get
        {
            if (!_engineSounSource)
            {
                _engineSounSource = GetComponent<AudioSource>();
            }

            return _engineSounSource;
        }
    }

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
        if (_wheelsController) _wheelConstant = 2 * Mathf.PI * _wheelsController.Radius * 60;
    }

    private void FixedUpdate()
    {
        CarMovementHandler();
    }

    private void OnGUI()
    {
        GUI.Button(new Rect(10, 10, 100, 50), _currentSpeed.ToString("F"));
        GUI.Button(new Rect(10, 60, 100, 50), _torque.ToString("F"));
        GUI.Button(new Rect(10, 110, 100, 50), _currentGear.ToString());
    }

    #endregion

    #region PRIVATE METHODS

    private void OnSpeedUpStart()
    {
        PlayIfNotPlaying(_engineSounSourceProp, _onSpeedUp);
    }

    private void OnSpeedUpEnd()
    {
        PlayIfNotPlaying(_engineSounSourceProp, _engineDefault);
    }

    private void PlayIfNotPlaying(AudioSource source, AudioClip clip)
    {
        if(!source || (source.clip == clip && source.isPlaying)) return;
        source.clip = clip;
        source.Play();
    }

    private void CarMovementHandler()
    {
        if(!_wheelsController || !_wheelsController.IsWheelsReady) throw new NullReferenceException("_wheelsController is null.");

        _currentSpeed = _wheelConstant * _wheelsController.RPM / 1000;
        _torque = Torque;
        _wheelsController.Torque(_torque);
    }

    #endregion

    #region PUBLIC METHODS

    public void OnGazDown()
    {
        _timeDrivePressed = 0;
        _timeEventDriveWasPressed = Time.time;
        OnSpeedUpStart();
    }

    public void OnGaz()
    {
        if (_timeEventDriveWasPressed == -1)
            _timeEventDriveWasPressed = Time.time;
        _timeDrivePressed = Time.time - _timeEventDriveWasPressed;
    }

    public void OnGazUp()
    {
        _timeDrivePressed = -1;
        _timeEventDriveWasPressed = -1;
        OnSpeedUpEnd();
    }

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        if(_rigidbody) _rigidbody.velocity = Vector3.zero;
        _currentGear = EngineGear.N;
    }

    #endregion
}

