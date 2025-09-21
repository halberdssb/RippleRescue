using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraTestControls : MonoBehaviour
{
    private Vector2 _cameraInput;
    private Vector2 _moveInput;
    private float _riseInput;
    
    public delegate void ToggleControlsDisplayPressedDelegate();
    public ToggleControlsDisplayPressedDelegate onToggleControlsDisplayPressed;

    [SerializeField] private float _cameraSensitivity;

    [SerializeField] private float _moveSpeed;
    
    private float _sensitivityMod = 0.001f; // modifier for move and rotate speeds to allow inspector values to be larger/more typical numbers


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        RotateCamera();
        MoveCamera();
    }

    // handles object rotation
    private void RotateCamera()
    {
        Vector3 rotationValue = _cameraInput * _cameraSensitivity * _sensitivityMod;
        
        // invert y axis rotation
        rotationValue.y *= -1;
        
        transform.Rotate(rotationValue.y, rotationValue.x, 0);

        // clamp z axis
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }

    // handles object movement on x, y, and z axes
    private void MoveCamera()
    {
        Vector3 localXMovement = _moveInput.x * transform.right;
        Vector3 localYMovement = _riseInput * transform.up;
        Vector3 localZMovement = _moveInput.y * transform.forward;
        
        Vector3 movement = localXMovement + localYMovement + localZMovement;
        movement *= _moveSpeed * _sensitivityMod;
        
        transform.position += movement;
    }

    #region Input Action Events
    
    private void OnRotateCamera(InputValue value)
    {
        _cameraInput = value.Get<Vector2>();
    }

    private void OnMoveCamera(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void OnRiseFallCamera(InputValue value)
    {
        _riseInput = value.Get<float>();
    }

    private void OnToggleControlsDisplay(InputValue value)
    {
        if (value.isPressed)
        {
            onToggleControlsDisplayPressed?.Invoke();
        }
    }
    
    #endregion
}
