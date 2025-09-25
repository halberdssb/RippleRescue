using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private PlayerControls _controls;

    [SerializeField]
    private LineRenderer _line;

    private List<Vector3> _linePoints = new List<Vector3>();

    private float _distanceBetweenPoints;
    private float _maxLineDistance;

    [SerializeField]
    private GameObject _waterPlane;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controls = GetComponent<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Checks if touch input has moved far enough to draw another point
    private bool CanNewPointBeDrawn()
    {
        return true;
        //if (_controls.TouchPosition)
    }

    // Gets distance between two points
    private float GetDistanceBetweenCurrentTouchAndLastLinePoint()
    {
        return 0;// _controls.touch
    }

    // Converts touch screen position to position on water plane
    private Vector3 ConvertTouchToWorldSpace(Vector2 touchInput)
    {
        Vector3 worldSpaceTouchPosition = Camera.main.ScreenToWorldPoint(touchInput);
        Vector3 convertedWorldSpacePositionOnscreen = Camera.main.WorldToScreenPoint(worldSpaceTouchPosition);

        //Vector3 finalConvertedPosition = new Vector3(convertedWorldSpacePositionOnscreen.x, convertedWorldSpacePositionOnscreen.y, )
        return Vector3.zero;
    }
}
