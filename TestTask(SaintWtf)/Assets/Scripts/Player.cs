using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    private float _turnSmoothVelocity;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Joystick _joystick;

    [Header("References")] 
    [SerializeField] private CameraFollow _camera;
    
    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if(_joystick.Direction == Vector3.zero)
            return;

        float targetAngle = Mathf.Atan2(_joystick.Direction.x, _joystick.Direction.z) * Mathf.Rad2Deg +
                            _camera.transform.eulerAngles.y;
        
        Vector3 movementVector = Quaternion.Euler(0 , targetAngle , 0) * Vector3.forward * _speed;
        _rigidbody.velocity = movementVector * Time.fixedDeltaTime;
        
        float lookAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity , _turnSmoothTime);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x , lookAngle , transform.eulerAngles.z);

    }
}
