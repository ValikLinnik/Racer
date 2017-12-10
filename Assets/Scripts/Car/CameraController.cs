using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{	
    #region SERIALIZE FIELDS

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Transform _viewTarget;

    [SerializeField,Range(0.1f, 20f)]
    private float _movingSpeed = 1f;

    #endregion

    #region UNITY EVENTS

    private void FixedUpdate()
    {
        if (!_viewTarget) return;
        transform.position = Vector3.Lerp(transform.position, _target.position, Time.deltaTime * _movingSpeed);
        transform.LookAt(_viewTarget.position);
    }

    #endregion
}
