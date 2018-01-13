using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Injection;
using Game.Data;
using UnityEngine.SceneManagement;
using System;

public class LevelManager : InjectorBase<LevelManager> 
{	
    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    [Inject]
    private LevelUIManager _levelUIManager;

    [Inject]
    private CargoManager _cargoManager;

    [Inject]
    private CarController _carController;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Timer _timer;

    [SerializeField]
    private LevelConfig _levelConfig;

    #endregion

    #region PUBLIC PROPERTIES

    public float LevelTime
    {
        get
        {
            if (!_timer) throw new NullReferenceException("Timer is null.");
            return _timer.CurrentTime;
        }
    }

    public int ObjToGrab
    {
        get
        {
            if(!_cargoManager) throw new NullReferenceException("CargoManager is null.");
            return _cargoManager.ObjToGrabCount;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private Vector3 _carStartPosition;
    private Quaternion _carStartRotation;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if(_carController) 
        {
            _carStartPosition = _carController.transform.position;
            _carStartRotation = _carController.transform.rotation;
        }

        if (_stateManager) _stateManager.OnStateChanged += OnStateChanged;
        if(_levelUIManager) 
        {
            _levelUIManager.OnMainMenuButtonClick += OnMainMenuButtonClick;
            _levelUIManager.OnRestartLevelClick += OnRestartLevelClick;
        }

        if(_cargoManager) _cargoManager.OnAllDone += OnAllDone;

        StartLevel();
    }

    private void OnDestroy()
    {
        if (_stateManager) _stateManager.OnStateChanged -= OnStateChanged;

        if(_levelUIManager) 
        {
            _levelUIManager.OnMainMenuButtonClick -= OnMainMenuButtonClick;
            _levelUIManager.OnRestartLevelClick -= OnRestartLevelClick;
        }

        if(!Context.Instance.IsNull()) Context.Instance.Dispose();
        Time.timeScale = 1;
    }

    void OnMainMenuButtonClick ()
    {
        SceneManager.LoadScene(GameConfig.MainMenuSceneName);     
    }

    #endregion

    #region PRIVATE METHODS

    private void StartLevel()
    {
        if(_stateManager) _stateManager.SetState(GameState.Play);

        if(_carController) 
        {
            _carController.transform.position = _carStartPosition;
            _carController.transform.rotation = _carStartRotation;
        }

        if(_timer) _timer.ON();
        if(_cargoManager) _cargoManager.StartLevel();
    }

    private void OnRestartLevelClick ()
    {
        if(_carController) _carController.Dispose();
        if(_timer) _timer.OFF();
        if(_stateManager) _stateManager.SetState(GameState.None);

        StartLevel();
    }

    private void OnStateChanged(GameState current, GameState previous)
    {
        var stopCondition = current == GameState.Pause || current == GameState.GameOver;
        if(stopCondition)  _timer.Pause();
        else _timer.Resume();

        Time.timeScale = stopCondition ? 0 : 1;
    }

    private void OnAllDone ()
    {
        if(!_timer) return;

        var time = _timer.CurrentTime;
        int starCount = 1;
        starCount = (time <= _levelConfig.TimeToReach3Stars) ? 3 : (time <= _levelConfig.TimeToReach2Stars) ? 2 : 1;
        if(_levelUIManager) _levelUIManager.SetGameOverMessage(string.Format(GameConfig.GameOverText, starCount, Timer.GetFormattedTime(time)));
        if(_stateManager) _stateManager.SetState(GameState.GameOver);
    }

    #endregion
}
