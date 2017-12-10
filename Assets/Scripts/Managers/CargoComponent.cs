using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CargoComponent : MonoBehaviour 
{	
    #region EVENTS HANDLERS

    public event Action<CargoComponent> OnGrab;

    private void OnGrabHandler()
    {
        if (OnGrab != null) OnGrab(this);
    }

    #endregion

    #region PUBLIC PROPERTIES

    public bool IsDone
    {
        get
        {
            return !gameObject.activeSelf;
        }

        set
        {
            gameObject.SetActive(!value);
        }
    }

    public bool IsNext
    {
        get;
        set;
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(!IsNext || !other.CompareTag(GameConfig.PlayerTag)) return;
        OnGrabHandler();
    }
}
