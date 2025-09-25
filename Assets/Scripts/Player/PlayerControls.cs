using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private Vector2 _touchPosition;
    private bool _touchPressed;

    public Vector2 TouchPosition
    {
        get { return _touchPosition; }
    }

    public bool TouchPressed
    { 
        get { return _touchPressed; } 
    }
    
    private void OnTouchPosition(InputValue value)
    {
        _touchPosition = value.Get<Vector2>();
    }

    private void OnTouchPressed(InputValue value)
    {
        _touchPressed = value.isPressed;
    }
}
