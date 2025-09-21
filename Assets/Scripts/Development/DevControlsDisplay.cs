using UnityEngine;

public class DevControlsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _controlsDisplay;

    [Space] [SerializeField] private CameraTestControls _controls;
    
    void Start()
    {
        _controls.onToggleControlsDisplayPressed += ToggleControlsDisplay;
    }

    private void ToggleControlsDisplay()
    {
        _controlsDisplay.SetActive(!_controlsDisplay.activeSelf);
    }
}
