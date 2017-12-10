using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorSwitcherComponent : MonoBehaviour 
{	
    #region SERIALIZE FIELDS

    [SerializeField]
    private MaskableGraphic _graphic;

    [SerializeField]
    private Color _targetColor;

    [SerializeField, Range(.01f, 2f)]
    private float _speed = 1f;

    #endregion

    #region PRIVATE FIELDS

    public Color Color
    {
        get
        {
            if (!_graphic) throw new NullReferenceException("MaskableGraphic is null.");
            return _graphic.color;
        }

        set
        {
            if (!_graphic) throw new NullReferenceException("MaskableGraphic is null.");
            _graphic.color = value;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private Color _defaultColor;
    private WaitForFixedUpdate _fixedUpdate;

    #endregion

    #region UNITY EVENTS

    private void Awake()
    {
        _defaultColor = Color;
        _fixedUpdate = new WaitForFixedUpdate();
    }

    private void OnEnable()
    {
        Color = _defaultColor;
        StopAllCoroutines();
        StartCoroutine(ChangeColorHandler());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion

    #region PRIVATE METHODS

    private IEnumerator ChangeColorHandler()
    {
        if (Color == _targetColor) gameObject.SetActive(false);
        yield return _fixedUpdate;
        Color = Color.Lerp(Color, _targetColor, Time.fixedDeltaTime * _speed);
        StartCoroutine(ChangeColorHandler());
    }

    #endregion
}
