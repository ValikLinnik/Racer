using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Injection;
using UnityEngine.UI;
using Game.Data;
using UnityEngine.SceneManagement;
using System;

public class LevelUIManager : InjectorBase<LevelUIManager> 
{
    #region EVENTS HANDLERS

    public event Action OnMainMenuButtonClick;

    private void OnMainMenuButtonClickHandler()
    {
        if (OnMainMenuButtonClick != null) OnMainMenuButtonClick();
    }

    public event Action OnRestartLevelClick;

    private void OnRestartLevelClickHandler()
    {
        if (OnRestartLevelClick != null) OnRestartLevelClick();
    }

    #endregion

    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    [Inject]
    private LevelManager _levelManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Button _pauseButton;

    [SerializeField]
    private PausePopUp _pausePopUp;

    [SerializeField]
    private ConfirmationPopUp _confirmationPopUp;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Button _restartButton;

    [SerializeField]
    private Text _levelTime;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if (_pauseButton) _pauseButton.onClick.AddListener(OnPauseButtonClick);

        if (_pausePopUp) 
        {
            _pausePopUp.OnResumeButtonClick += OnResumeButtonClick; 
            _pausePopUp.OnMainMenuButtonClick += OnMainMenuClick;
            _pausePopUp.OnRestartButtonClick += OnRebootLevelClick;
        }

        if(_restartButton) _restartButton.onClick.AddListener(OnRestartLevelClickHandler);

    }

    private void FixedUpdate()
    {
        if(_levelTime && _levelManager)
        {
            _levelTime.text = Timer.GetFormattedTime(_levelManager.LevelTime);
        }
    }

    private void OnDestroy()
    {
        if (_pauseButton) _pauseButton.onClick.RemoveAllListeners();

        if (_pausePopUp) 
        {
            _pausePopUp.OnResumeButtonClick -= OnResumeButtonClick; 
            _pausePopUp.OnMainMenuButtonClick -= OnMainMenuClick;
            _pausePopUp.OnRestartButtonClick -= OnRebootLevelClick;
        }

        if(_restartButton) _restartButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region PUBLIC METHODS

    public void SetGameOverMessage(string text)
    {
        if (_gameOverText) _gameOverText.text = text;
    }

    #endregion

    #region PRIVATE METHODS

    private void OnPauseButtonClick()
    {
        if (_stateManager) _stateManager.SetState(GameState.Pause);

        if(_pausePopUp && _levelManager) 
        {   
            var data = new PausePopUpData(Timer.GetFormattedTime(_levelManager.LevelTime), _levelManager.ObjToGrab.ToString());
            _pausePopUp.SetData(data);
            _pausePopUp.Show();
        }
    }

    private void OnResumeButtonClick()
    {
        HideAllPopUps();
        if(_stateManager) _stateManager.SetState(GameState.Resume);
    }

    private void OnMainMenuClick()
    {
        if(_pausePopUp) _pausePopUp.Hide();
        if(!_confirmationPopUp) throw new NullReferenceException("Confirmation pop-up is null.");

        var data = new ConfirmationData(GameConfig.SureText, OnMainMenuButtonClickHandler, OnResumeButtonClick);

        _confirmationPopUp.SetData(data);
        _confirmationPopUp.Show();
    }

    private void OnRebootLevelClick()
    {
        if(_pausePopUp) _pausePopUp.Hide();
        if(!_confirmationPopUp) throw new NullReferenceException("Confirmation pop-up is null.");

        var data = new ConfirmationData(GameConfig.SureText,
            ()=>
            {   
                if(_confirmationPopUp) _confirmationPopUp.Hide();
                OnRestartLevelClickHandler();
            },
            OnResumeButtonClick);

        _confirmationPopUp.SetData(data);
        _confirmationPopUp.Show();
    }

    private void HideAllPopUps()
    {
        if(_confirmationPopUp) _confirmationPopUp.Hide();
        if(_pausePopUp) _pausePopUp.Hide();
    }

    #endregion
}
