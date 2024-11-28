using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGMoving : MonoBehaviour
{

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _jumpPower = 5.0f;
    [SerializeField] private float _speedRun = 10.0f;
    [SerializeField] private float _speedSit = 2.0f;

    private Vector3 _walkDirection;
    private Vector3 _velocity;
    private float _speedWalk;

    private CharacterController _characterController;

    void Start()
    {
        _speedWalk = _speed;
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {

        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");

        _walkDirection = transform.forward * vertical_input + transform.right * horizontal_input;

        Jump(_characterController.isGrounded && Input.GetKey(KeyCode.Space));
        ChangeMoveSpeed(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl));

    }

    private void FixedUpdate()
    {

        Walk(_walkDirection);
        Gravity(_characterController.isGrounded);

    }

    private void Walk(Vector3 direction)
    {
        _characterController.Move(direction * _speed * Time.fixedDeltaTime);
    }

    private void Gravity(bool isGrounded)
    {
        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = -1f;
        }
        _velocity.y -= _gravity * Time.fixedDeltaTime;
        _characterController.Move(_velocity * Time.fixedDeltaTime);
    }

    private void Jump(bool canJump)
    {
        if (canJump)
        {
            _velocity.y = _jumpPower;
        }
    }

    private void ChangeMoveSpeed(bool changeMoveSpeed)
    {
        if (Input.GetKey(KeyCode.LeftShift))
            _speed = changeMoveSpeed ? _speedRun : _speedWalk;
        else
        {
            _characterController.height = changeMoveSpeed ? 1f : 2f;
            _speed = changeMoveSpeed ? _speedSit : _speedWalk;
        }
        
    }

}
