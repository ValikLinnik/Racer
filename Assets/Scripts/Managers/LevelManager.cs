using System;
using Game.Data;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private AchievementsManager _achievementsManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Level _level;

    [SerializeField]
    private Timer _timer;

    [SerializeField]
    private LevelConfig _levelConfig;

    #endregion

    #region PUBLIC PROPERTIES

    public Level Level
    {
        get
        {
            return _level;
        }
    }

    public int LevelIndex
    {
        get
        {
            return (int)_level;
        }
    }

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
    #endregion

    #region UNITY EVENTS

    private void Start()
    {
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

    private void OnMainMenuButtonClick ()
    {
        SceneManager.LoadScene(GameConfig.MainMenuSceneName);     
    }

    #endregion

    #region PRIVATE METHODS

    private void StartLevel()
    {
        if(_stateManager) _stateManager.SetState(GameState.Play);
        if(_timer) _timer.ON();
        if(_cargoManager) _cargoManager.StartLevel();
    }

    private void OnRestartLevelClick ()
    {
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
        if(_achievementsManager) _achievementsManager.SetLevelTime(time);
        int starCount = 1;
        starCount = (time <= _levelConfig.TimeToReach3Stars) ? 3 : (time <= _levelConfig.TimeToReach2Stars) ? 2 : 1;
        var bestTime = Timer.GetFormattedTime(_achievementsManager ? _achievementsManager.BestTime : 0);
        if(_levelUIManager) _levelUIManager.SetGameOverMessage(string.Format(GameConfig.GameOverText, starCount, Timer.GetFormattedTime(time), bestTime));
        if(_stateManager) _stateManager.SetState(GameState.GameOver);
    }

    #endregion
}

public enum Level
{
    First = 0,
    Second = 1,
    Third = 2
}
