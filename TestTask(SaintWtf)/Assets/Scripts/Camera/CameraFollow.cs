using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 0.1f;
    
    [SerializeField] private bool _defineOffsetOnStart = true;
    [SerializeField] private Vector3 _offset;
    private Vector3 _currentOffset;

    void Start()
    {
        if (_defineOffsetOnStart)
        {
            _offset = transform.position - _target.position;
        }
        
        _currentOffset = _offset;
    }

    void FixedUpdate()
    {
        Vector3 targetPositionWithOffset = _target.position + _currentOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPositionWithOffset, _smoothSpeed);

        transform.position = smoothedPosition;
    }
}
