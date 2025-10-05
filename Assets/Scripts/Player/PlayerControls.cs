using UnityEngine;
using UnityEngine.InputSystem;

/*
 *  Receives messages from Player Input and maps them to variables
 *
 *  Jeff Stevenson
 *  9.25.25
 */
public class PlayerControls : MonoBehaviour
{
    private Vector2 _touchPosition;
    private Vector2 _touchDelta;
    private bool _touchPressed;

    public Vector2 TouchPosition
    {
        get { return _touchPosition; }
    }

    public Vector2 TouchDelta
    {
        get { return _touchDelta; }
    }

    public bool TouchPressed
    { 
        get { return _touchPressed; } 
    }
    
    private void OnTouchPosition(InputValue value)
    {
        _touchPosition = value.Get<Vector2>();
    }

    private void OnTouchDelta(InputValue value)
    {
        _touchDelta = value.Get<Vector2>();
    }
    
    private void OnTouchPressed(InputValue value)
    {
        _touchPressed = value.isPressed;
    }
}
