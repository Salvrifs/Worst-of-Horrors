using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speedWalk = 5.0f;
    [SerializeField] private float _speedCrouch = 2.5f; 
    [SerializeField] private float _jumpHeight = 2.0f; 
    [SerializeField] private float gravity = -9.81f; 

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 100.0f;
    
    private CharacterController _characterController;
    private Transform _cameraTransform;
    private Vector3 _walkDirection;
    private float _ySpeed = 0f; 
    private bool _isCrouching = false; 
    
    private float _xRotation = 0f;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _cameraTransform = Camera.main.transform; 

        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f); 
        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX); 

        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        _walkDirection = transform.right * x + transform.forward * z;

        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _isCrouching = false;   
        }

        
        if (_characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _ySpeed = Mathf.Sqrt(_jumpHeight * -2f * gravity);
            }
        }

        
        _ySpeed += gravity * Time.deltaTime;
    }

    void FixedUpdate()
    {
        
        Vector3 move = _walkDirection * (_isCrouching ? _speedCrouch : _speedWalk);
        move.y = _ySpeed; 
        _characterController.Move(move * Time.fixedDeltaTime);
    }
}
