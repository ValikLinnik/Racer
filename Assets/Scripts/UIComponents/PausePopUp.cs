using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PausePopUp : PopBase 
{
    #region EVENTS HANDLERS

    public event Action OnResumeButtonClick;

    private void OnResumeButtonClickHandler()
    {
        if (OnResumeButtonClick != null) OnResumeButtonClick();
    }

    public event Action OnMainMenuButtonClick;

    private void OnMainMenuButtonClickHandler()
    {
        if (OnMainMenuButtonClick != null) OnMainMenuButtonClick();
    }

    public event Action OnRestartButtonClick;

    private void OnRestartButtonClickHandler()
    {
        if (OnRestartButtonClick != null) OnRestartButtonClick();
    }

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField]
    private Button _resumeButton;

    [SerializeField]
    private Button _toMainMenuButton;

    [SerializeField]
    private Button _restartButton;

    [SerializeField]
    private Text _bestTime;

    [SerializeField]
    private Text _currentTime;

    [SerializeField]
    private Text _objToGrab;

    #endregion

    #region PRIVATE FIELDS

    private PausePopUpData _data;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if(_resumeButton) _resumeButton.onClick.AddListener(OnResumeButtonClickHandler);
        if(_toMainMenuButton) _toMainMenuButton.onClick.AddListener(OnMainMenuButtonClickHandler);
        if(_restartButton) _restartButton.onClick.AddListener(OnRestartButtonClickHandler);
    }

    private void OnDestroy()
    {
        if(_resumeButton) _resumeButton.onClick.RemoveAllListeners();
        if(_toMainMenuButton) _toMainMenuButton.onClick.RemoveAllListeners();
        if(_restartButton) _restartButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region implemented abstract members of PopBase

    public override void SetData(PopUpData data)
    {
        if(!data) 
        {
            Debug.LogFormat("<size=18><color=olive>{0}</color></size>", "Pause Pop-up data is null");
            Hide();
            return;
        }

        _data = data.ToType<PausePopUpData>();

        if(!_data) throw new InvalidCastException("Cannot cast data.");

        if(_currentTime) _currentTime.text = _data.LevelTime;
        if(_objToGrab) _objToGrab.text = _data.ObjToGrad;
    }

    #endregion
}

class PausePopUpData:PopUpData
{
    public PausePopUpData()
    {
        
    }

    public PausePopUpData(string levelTime, string objToGrab)
    {
        LevelTime = levelTime;
        ObjToGrad = objToGrab;
    }

    public string LevelTime
    {
        get;
        set;
    }

    public string ObjToGrad
    {
        get;
        set;
    }
}
