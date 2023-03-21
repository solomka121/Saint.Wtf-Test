using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Joystick _joystick;
    
    // [Header("Movement")]
    
    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if(_joystick.Direction == Vector3.zero)
            return;

        Vector3 movementVector = _joystick.Direction * _speed * Time.fixedDeltaTime;
        
        _rigidbody.MovePosition(_rigidbody.position + movementVector);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movementVector),
            _rotationSpeed * Time.deltaTime);
    }
}
