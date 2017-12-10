using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmationPopUp : PopBase 
{
    #region SERIALIZE FIELDS

    [SerializeField]
    private Text _message;

    [SerializeField]
    private Button _yesButton;

    [SerializeField]
    private Button _noButton;

    #endregion

    #region UNITY EVENTS

    private void Start()
    {
        if(_yesButton) _yesButton.onClick.AddListener(OnYesButtonClickHandler); 
        if(_noButton) _noButton.onClick.AddListener(OnNoButtonClickHandler);
    }

    private void OnDestroy()
    {
        if(_yesButton) _yesButton.onClick.RemoveAllListeners();    
        if(_noButton) _noButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region PRIVATE FIELDS

    private ConfirmationData _data;

    #endregion

    #region PRIVATE METHODS

    private void OnYesButtonClickHandler()
    {
        if(_data && !_data.YesAction.IsNull()) _data.YesAction();
    }

    private void OnNoButtonClickHandler()
    {
        if(_data && !_data.NoAction.IsNull()) _data.NoAction();
    }

    #endregion

    #region implemented abstract members of PopBase

    public override void SetData(PopUpData data)
    {
        if(!data) 
        {
            Debug.LogFormat("<size=18><color=olive>{0}</color></size>", "Data is null.");
            Hide();
            return;
        }

        _data = data.ToType<ConfirmationData>();

        if(!_data) throw new InvalidCastException("Cannot cast data.");

        if(_message) _message.text = _data.Message;
    }

    #endregion
}

public class ConfirmationData : PopUpData
{
    public ConfirmationData()
    {
        
    }

    public ConfirmationData(string message, Action yesAction, Action noAction)
    {
        Message = message;
        YesAction = yesAction;
        NoAction = noAction;
    }

    public string Message
    {
        get;
        set;
    }

    public Action YesAction
    {
        get;
        set;
    }

    public Action NoAction
    {
        get;
        set;
    }
}
