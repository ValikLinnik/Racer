using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Injection;
using System;

public class CargoManager : InjectorBase<CargoManager> 
{	
    #region EVENTS HANDLERS

    public event Action OnAllDone;

    private void OnAllDoneHandler()
    {
        if (OnAllDone != null) OnAllDone();
    }

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private CargoComponent[] _cargos;

    [SerializeField]
    private GameObject _animationArrow;

    [SerializeField]
    private WayPointComponent _wayPointComponent;

    #endregion

    #region PRIVATE FIELDS

    private int _lastPickedItemIndex;
    private int _leftItemsCount;

    #endregion

    #region PUBLIC PROPERTIES

    public int ObjToGrabCount
    {
        get
        {
            return _leftItemsCount;
        }
    }

    #endregion

    private void Start()
    {
        if(_cargos.IsNullOrEmpty()) throw new Exception("Cargos is null or empty");

        foreach (var item in _cargos)
        {
            if(!item) continue;
            item.OnGrab += OnGrab;
        }
    }

    private void OnDestroy()
    {
        foreach (var item in _cargos)
        {
            if(!item) continue;
            item.OnGrab -= OnGrab;
        }
    }

    private void OnGrab (CargoComponent cargo)
    {
        int index = _lastPickedItemIndex + 1;

        if(!cargo || !cargo.IsNext || _cargos.IsNullOrEmpty() || _cargos.IndexIsNotInRange(index) || _cargos[index] != cargo) 
        {
            Debug.LogFormat("<size=18><color=olive>{0}</color></size>", "Error check cargo.");
            return;
        }

        cargo.IsDone = true;
        _lastPickedItemIndex++;
        SetNextItem();
    }

    public void StartLevel()
    {
        if(_cargos.IsNullOrEmpty()) throw new Exception("Cargos is null or empty");

        foreach (var item in _cargos)
        {
            if(!item) throw new NullReferenceException("Null item! Check it!");
            item.IsDone = false;
            item.IsNext = false;
        }

        _lastPickedItemIndex = -1;
        SetNextItem();
    }

    private void SetNextItem()
    {
        _leftItemsCount = _cargos.Length - (_lastPickedItemIndex + 1);
        if(_leftItemsCount <= 0)
        {
            if(_animationArrow) _animationArrow.SetActive(false);
            OnAllDoneHandler();
            return;
        }

        if(_cargos.IsNullOrEmpty()) throw new Exception("Cargos is null or empty");
        int index = _lastPickedItemIndex + 1;

        if(_cargos.IndexIsNotInRange(index)) 
        {
            Debug.LogFormat("<size=18><color=olive>{0}</color></size>", "Incorrect index.");
            return;
        }

        if(_wayPointComponent) _wayPointComponent.Target = _cargos[index].transform;

        _cargos[index].IsNext = true;
        if(_animationArrow) 
        {
            _animationArrow.SetActive(true);
            _animationArrow.transform.position = _cargos[index].transform.position;
        }
    }
}
