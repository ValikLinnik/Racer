using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class GameMenuManager : MonoBehaviour 
{
    #region SERIALIZE FIELDS

    [SerializeField]
    private Button _startFirstLevelButton;

    [SerializeField]
    private Button _startSecondLevelButton;

    [SerializeField]
    private Button _startThirdLevelButton;

    [SerializeField]
    private GameObject _loadingWrapper;

    [SerializeField]
    private Material _skyBoxMaterial;

    [SerializeField, Range(.01f,1f)]
    private float _skyRotationSpeed = .01f;

    #endregion

    private string _rotatinFieldName = "_Rotation";
    private float _skyBoxRotationVal;
    private float _defaultSkyBoxRotation;

    #region UNITY EVENTS

    private void Awake()
    {
        if (_loadingWrapper) _loadingWrapper.SetActive(false);
        if (_startFirstLevelButton) _startFirstLevelButton.onClick.AddListener(OnFirstLevelLoadClickHandler);
        if (_startSecondLevelButton) _startSecondLevelButton.onClick.AddListener(OnSecondLevelLoadClickHandler);
        if (_startThirdLevelButton) _startThirdLevelButton.onClick.AddListener(OnThirdLevelLoadClickHandler);

        _defaultSkyBoxRotation = SkyBoxRotation;
    }

    private void LateUpdate()
    {
        SkyBoxRotationHandler();
    }

    private void OnDestroy()
    {
        if (_startFirstLevelButton) _startFirstLevelButton.onClick.RemoveAllListeners();
        if (_startSecondLevelButton) _startSecondLevelButton.onClick.RemoveAllListeners();
        if (_startThirdLevelButton) _startThirdLevelButton.onClick.RemoveAllListeners();
        SetSkyBoxRotation(_defaultSkyBoxRotation);
    }

    #endregion

    #region PRIVATE METHODS

    private void SkyBoxRotationHandler()
    {
        _skyBoxRotationVal += _skyRotationSpeed * Time.deltaTime;
        _skyBoxRotationVal = _skyBoxRotationVal > 360f ? 0 : _skyBoxRotationVal;
        SkyBoxRotation = _skyBoxRotationVal;
    }

    private float SkyBoxRotation
    {
        set
        {
            if (!_skyBoxMaterial) throw new NullReferenceException("SkyBoxMaterial is null.");
            _skyBoxMaterial.SetFloat(_rotatinFieldName, value);
        }

        get
        {
            if (!_skyBoxMaterial) throw new NullReferenceException("SkyBoxMaterial is null.");
            return _skyBoxMaterial.GetFloat(_rotatinFieldName);
        }
    }

    private void SetSkyBoxRotation(float val)
    {
        if (!_skyBoxMaterial) throw new NullReferenceException("SkyBoxMaterial is null.");
        _skyBoxMaterial.SetFloat(_rotatinFieldName, val);
    }

    private void OnFirstLevelLoadClickHandler()
    {
        LoadLevel(GameConfig.FirstLevelSceneName);
    }

    private void OnSecondLevelLoadClickHandler()
    {
        LoadLevel(GameConfig.SecondLevelSceneName);
    }

    private void OnThirdLevelLoadClickHandler()
    {
        LoadLevel(GameConfig.ThirdLevelSceneName);
    }

    private void LoadLevel(string levelName)
    {
        if (_loadingWrapper) _loadingWrapper.SetActive(true);
        SceneManager.LoadScene(levelName);   
    }

    #endregion
}
