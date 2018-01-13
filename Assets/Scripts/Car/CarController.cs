using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Injection;

public class CarController : InjectorBase<CarController>, IDisposable 
{
    #region SERIALIZE FIELDS

    [SerializeField]
    private CarEngine _engine;

    [SerializeField]
    private WheelsController _wheelsController;

    [SerializeField]
    private bool _isFourWheelDrive;

    #endregion

    #region PRIVATE FIELDS

    private bool _isBreaking;
    private float _brakingTorque = 500;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if(_wheelsController) _wheelsController.IsFourWheelDrive = _isFourWheelDrive;      
        if(_engine) _engine.IsFourWheelDrive = _isFourWheelDrive;
    }

    private void LateUpdate()
    {
        if(!_engine) throw new NullReferenceException("engine is null.");
        _isBreaking = Input.GetKey(KeyCode.Space);
        _engine.IsBreaking = _isBreaking;

        var forward = Input.GetKeyDown(KeyCode.UpArrow);
        var backward = Input.GetKeyDown(KeyCode.DownArrow);

        if (forward || backward)
        {
            _engine.EngineGear = backward ? EngineGear.R : EngineGear.D;
            _engine.OnGazDown();
            return;
        }

        forward = Input.GetKey(KeyCode.UpArrow);
        backward = Input.GetKey(KeyCode.DownArrow);

        if (forward || backward)
        {   
            _engine.EngineGear = backward ? EngineGear.R : EngineGear.D;
            _engine.OnGaz();
        }

        forward = Input.GetKeyUp(KeyCode.UpArrow);
        backward = Input.GetKeyUp(KeyCode.DownArrow);

        if(forward || backward)
        {
            _engine.OnGazUp();
        }   
    }

    private void FixedUpdate()
    {
        CarMovementHandler();
    }

    #endregion

    private void CarMovementHandler()
    {
        if(!_wheelsController || !_wheelsController.IsWheelsReady) throw new NullReferenceException("WheelsController is null.");

        var newSteer = Input.GetAxis("Horizontal");
        _wheelsController.CarMovementHandler(newSteer);
        var val = _isBreaking ? _brakingTorque : 0;
        _wheelsController.Break(val);
    }
	
    #region IDisposable implementation
    public void Dispose()
    {
        _isBreaking = false;
        if(_engine) _engine.Dispose();
        if(_wheelsController) _wheelsController.Dispose();
    }
    #endregion
}
