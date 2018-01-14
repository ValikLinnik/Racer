using System;
using System.Collections.Generic;
using Game.Data;
using Injection;
using UnityEngine;

public class ReplayManager : InjectorBase<ReplayManager>   
{	
    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    [Inject]
    private CarController _carController;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private string _prefabName;

    [SerializeField,Range(0,10)]
    private float _distanceDiff = 5;

    #endregion

    #region SERIALIZE FIELDS

    public List<RaceSnapshot> CurrentRace
    {
        get
        {
            return _currentRace;
        }
    }

    public List<RaceSnapshot> BestRace
    {
        get
        {
            return _bestRace;
        }
        set
        {
            _bestRace = value;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private Transform _model;
    private List<RaceSnapshot> _currentRace = new List<RaceSnapshot>(1024);
    private List<RaceSnapshot> _bestRace;
    private float _raceStartTime;
    private bool _isShotingRace;
    private GameObject _modelPrefab;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if (_stateManager)
        {
            _stateManager.OnStateChanged += OnStateChanged;
            OnStateChanged(_stateManager.CurrentState, _stateManager.CurrentState);
        }
    }

    private void FixedUpdate()
    {
        if (_isShotingRace)
        {
            SetRaceSnapshot();
            if (!_bestRace.IsNullOrEmpty())
                UpdateModelPosition();
        }
    }

    private void OnDestroy()
    {
        if (_stateManager)
        {
            _stateManager.OnStateChanged -= OnStateChanged;
        }
    }

    #endregion

    #region PRIVATE METHODS

    private void OnStateChanged(Game.Data.GameState current, Game.Data.GameState previous)
    {
        if (current == GameState.Play)
        {
            _currentRace.Clear();
            _raceStartTime = Time.time;
            _isShotingRace = true;
        }

        if (current == GameState.GameOver)
        {
            _isShotingRace = false;
        }
    }

    private void SetRaceSnapshot()
    {
        if (!_carController)
            return;

        var lastSnap = !_currentRace.IsNullOrEmpty() ? _currentRace[_currentRace.Count - 1] : null;

        if (lastSnap)
        {
            var distance = Vector3.Distance(lastSnap.Position, _carController.transform.position);
            if (distance <= _distanceDiff)
                return;
        }

        var temp = new RaceSnapshot();
        temp.Time = Time.time - _raceStartTime;
        temp.Position = _carController.transform.position;
        temp.Rotation = _carController.transform.rotation;
        _currentRace.Add(temp);
    }

    private void UpdateModelPosition()
    {
        if (!_model)
            LoadModel();
        var time = Time.time - _raceStartTime;

        for (int i = 0; i < _bestRace.Count; i++)
        {
            var temp = _bestRace[i];
            if (!temp)
                continue;
            if (temp.Time > time)
            {
                if (i == 0)
                    continue;

                var nextSnap = temp;
                var previousSnap = _bestRace[i - 1];

                if (!nextSnap || !previousSnap)
                    continue;

                var progress = (time - previousSnap.Time) / (nextSnap.Time - previousSnap.Time);

                var difLength = Vector3.Distance(previousSnap.Position, nextSnap.Position) * progress;
                var newPosition = Vector3.MoveTowards(previousSnap.Position, nextSnap.Position, difLength);
                _model.position = newPosition;

                var difAngle = Quaternion.Angle(previousSnap.Rotation, nextSnap.Rotation) * progress;
                var newRotation = Quaternion.RotateTowards(previousSnap.Rotation, nextSnap.Rotation, difAngle);
                _model.rotation = newRotation;
                return;
            }
        }
    }

    private void LoadModel()
    {
        _modelPrefab = Resources.Load<GameObject>(_prefabName);
        if (!_modelPrefab)
        {
            throw new NullReferenceException("prefab is null.");
        }

        _model = _modelPrefab.transform.GetInstance();
        if (!_model)
        {
            throw new NullReferenceException("instance is null");
        }

        _model.parent = null;
        _model.gameObject.SetActive(true);
    }

    #endregion
}
