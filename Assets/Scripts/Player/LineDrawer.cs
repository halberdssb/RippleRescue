using System.Collections.Generic;
using UnityEngine;

/*
 *  Draws lines on water plane for player to follow
 *
 *  Jeff Stevenson
 *  9.25.25
 */

public class LineDrawer : MonoBehaviour
{
   
    private PlayerControls _controls;

    [SerializeField]
    private LineRenderer line;

    private List<Vector3> _linePoints = new List<Vector3>();

    [SerializeField] 
    private float startDrawDistance;
    [SerializeField]
    private float distanceBetweenPoints;

    private float maxLineDistance;

    [SerializeField]
    private GameObject waterPlane;

    [SerializeField] 
    private LayerMask raycastLayerMask;

    private bool _drawingLine;
    private bool _lineDrawingEnabled;
    
    private float _maxRaycastDistance = 100f;
    private float _distanceDraggedSinceLastPoint;
    private float _totalDistanceDrawn;
    private float _worldSpaceTouchDeltaMagnitude;
    private Vector3 _worldSpaceTouchLastFrame;
    private float _lineOffsetAbovePlayer = 0.2f; // distance on y axis the line will be above the player's height

    private float _sqrDistanceBetweenPoints;
    private float _sqrPlayerTouchDistance;

    private Camera _camera;
    private LineFollower _lineFollower;

    public Vector3[] LinePoints
    {
        get { return  _linePoints.ToArray(); }
    }

    void Start()
    {
        _controls = GetComponent<PlayerControls>();
        _lineFollower = GetComponent<LineFollower>();
        _camera = Camera.main;
        
        _sqrDistanceBetweenPoints = distanceBetweenPoints * distanceBetweenPoints;
        _sqrPlayerTouchDistance = startDrawDistance * startDrawDistance;
        
        // set to position on default because first point will always be on top of player
        _worldSpaceTouchLastFrame = transform.position;

        maxLineDistance = _lineFollower.MoveSpeed * WaterDrain.Instance.DrainTime;
    }

    void Update()
    {
        if (!_lineDrawingEnabled) return;
        
        // Start drawing if touch near ship and no line has been drawn yet
        if (_controls.TouchPressed && !_drawingLine && IsTouchWithinStartDrawRange() && !HasLineBeenDrawn())
        {
            _drawingLine = true;
            AddFirstPointToLine();
        }
        // Reset touch delta when line is stopped being drawn
        else if (!_controls.TouchPressed && _drawingLine)
        {
            StopDrawingLine();
            // Reset line if only 1 point or less
            if (LinePoints.Length <= 1)
            {
                ResetLine();
            }
        }
        // Check if max line distance has been drawn
        else if (IsLineAtOrPastMaxDrawDistance())
        {
            StopDrawingLine();
        }
        
        // Draw line
        if (_drawingLine && TryConvertTouchToWorldSpace(_controls.TouchPosition, out Vector3 worldSpaceTouchPoint))
        {
            // Update distance from last line point
            UpdateTouchDelta(worldSpaceTouchPoint);
            
            // Draw new point if enough distance has been drawn from last point
            if (CanNewPointBeDrawn())
            {
                //Debug.Log("Adding new point");
                AddPointToLine(worldSpaceTouchPoint);
                ResetTouchDelta();
            }
        }
    }
    
    // Enables/disables ability to draw lines
    public void SetLineDrawerActive(bool active)
    {
        _lineDrawingEnabled = active;
    }
    
    // Checks if the player is touching within start draw distance to the player
    private bool IsTouchWithinStartDrawRange()
    {
        if (TryConvertTouchToWorldSpace(_controls.TouchPosition, out Vector3 worldSpaceTouchPoint))
        {
            return GetSquareDistanceToPlayer(worldSpaceTouchPoint) < _sqrPlayerTouchDistance;
        }
        
        return false;
    }
    
    // Adds the touch delta to the distance drawn from last point
    private void UpdateTouchDelta(Vector3 worldSpaceTouchPosition)
    {
        _worldSpaceTouchDeltaMagnitude = (worldSpaceTouchPosition - _worldSpaceTouchLastFrame).magnitude; 
        _distanceDraggedSinceLastPoint += _worldSpaceTouchDeltaMagnitude;
        _worldSpaceTouchLastFrame = worldSpaceTouchPosition;
    }

    // Checks if touch input has moved far enough to draw another point
    private bool CanNewPointBeDrawn()
    {
        // always draw first point of line
        if (_linePoints.Count <= 0)
        {
            return true;
        }
        
        if (_distanceDraggedSinceLastPoint >= distanceBetweenPoints)
        {
            return true;
        }

        return false;
    }

    // Gets distance between two points
    private float GetSquareDistanceToPlayer(Vector3 worldSpaceTouchPoint)
    {
        return (transform.position - worldSpaceTouchPoint).sqrMagnitude;
    }

    // Tries to convert touch screen position to position on water plane if touch hits water plane and returns results
    private bool TryConvertTouchToWorldSpace(Vector2 touchInput, out Vector3 worldSpacePoint)
    {
        // get world position of touch input on level of player
        Vector3 touchPositionOnPlane = GetTouchPositionOnPlayerPlane(touchInput);
        
        // raycast to attempt to hit water plane
        Vector3 directionFromCameraToTouchOnPlane = (touchPositionOnPlane - _camera.transform.position).normalized;
        if (Physics.Raycast(_camera.transform.position, directionFromCameraToTouchOnPlane, out RaycastHit hitInfo, _maxRaycastDistance, raycastLayerMask))
        {
            // use local object y to ensure line is drawn on same plane as player
            Vector3 worldSpacePointOnPlane = new Vector3(hitInfo.point.x, transform.position.y + _lineOffsetAbovePlayer, hitInfo.point.z);
            worldSpacePoint = worldSpacePointOnPlane;
            return true;
        }
        
        worldSpacePoint = Vector3.zero;
        return false;
    }

    // Converts touch position to position on the water plane the player is on
    private Vector3 GetTouchPositionOnPlayerPlane(Vector2 touchInput)
    {
        float distanceToCamera = (transform.position - _camera.transform.position).magnitude;
        Vector3 touchPositionOnPlane = _camera.ScreenToWorldPoint(new Vector3(touchInput.x, touchInput.y, distanceToCamera)); 
        
        return touchPositionOnPlane;
    }
    
    // Adds a new point to the line renderer and draws it
    private void AddPointToLine(Vector3 pointToAdd)
    {
        // update line distance drawn if not first point - first point will always be distance 0 from player so don't need to add
        if (_linePoints.Count > 0)
        {
            _totalDistanceDrawn += GetDistanceFromLastPointToNewPoint(pointToAdd);
        }
        
        // update line renderer points
        if (!line.useWorldSpace)
        {
            pointToAdd -= line.transform.position;
        }
        _linePoints.Add(pointToAdd);
        line.positionCount = _linePoints.Count;
        line.SetPositions(_linePoints.ToArray());
        
        // reset distance from last point
        _distanceDraggedSinceLastPoint = 0;
    }
    
    // Adds the first point of the line at the player's position
    private void AddFirstPointToLine()
    {
        Vector3 startingPointPosition = new Vector3(transform.position.x, transform.position.y + _lineOffsetAbovePlayer, transform.position.z);
        AddPointToLine(startingPointPosition);
    }
    
    // Resets the touch delta from last drawn point
    private void ResetTouchDelta()
    {
        _distanceDraggedSinceLastPoint = 0;
    }
    
    // Gets distance from last point on line and new point to be added
    private float GetDistanceFromLastPointToNewPoint(Vector3 newPoint)
    {
        return (_linePoints[_linePoints.Count - 1] - newPoint).magnitude;
    }
    
    // Gets if max line length has been drawn
    private bool IsLineAtOrPastMaxDrawDistance()
    {
        return _totalDistanceDrawn >= maxLineDistance;
    }
    
    // Returns if the line has been partially drawn already and let go
    private bool HasLineBeenDrawn()
    {
        return (_totalDistanceDrawn > 0) && (!_drawingLine);
    }

    // Removes a point from the line renderer by index
    public void RemoveLinePointAtIndex(int index)
    {
        _linePoints.RemoveAt(index);
        line.SetPositions(_linePoints.ToArray());
    }
    
    // Stops drawing line and resets proper value
    private void StopDrawingLine()
    {
        ResetTouchDelta();
        _drawingLine = false;
    }
    
    // Resets the line and sets it ready to draw again
    public void ResetLine()
    {
        line.positionCount = 0;
        _linePoints.Clear();
        _drawingLine = false;
        _distanceDraggedSinceLastPoint = 0;
        _totalDistanceDrawn = 0;
        _worldSpaceTouchDeltaMagnitude = 0;
        _worldSpaceTouchLastFrame = Vector3.zero;
    }
}
