using DG.Tweening;
using UnityEngine;

/*
 *  Handles movement along a line for player or other entities
 *
 *  Jeff Stevenson
 *  9.25.25
 */

public class LineFollower : MonoBehaviour
{
    [SerializeField] 
    private float moveSpeed;

    [SerializeField] 
    private LayerMask obstacleLayerMask;

    private LineDrawer _lineDrawer;
    
    private Tween _moveTween;
    private Tween _turnTween;
    private Vector3[] _currentLinePoints;
    private int _nextPointIndex;

    private void Start()
    {
        _lineDrawer = GetComponent<LineDrawer>();
    }
    
    // Begins set movement along a line through all points
    public void MoveAlongLineToEnd()
    {
        if (_lineDrawer.LinePoints.Length <= 0) return;
        
        // get line points from line drawer
        _currentLinePoints = _lineDrawer.LinePoints;
        
        // start moving along points
        _nextPointIndex = 1;
        MoveToNextPointLooping(_currentLinePoints[_nextPointIndex]);
    }

    // Starts a tween to move toward the next point at a constant speed and cues movement to next point on tween complete
    private void MoveToNextPointLooping(Vector3 destination)
    {
        Vector3 distanceVectorToDestination = destination - transform.position;
        
        // start moving to next point
        float tweenTime = distanceVectorToDestination.magnitude / moveSpeed;
        _moveTween = transform.DOMove(destination, tweenTime);
        _moveTween.SetEase(Ease.Linear);
        
        // cue move to next point if there is one on tween complete
        _moveTween.onComplete += () =>
        {
            if (_nextPointIndex < _currentLinePoints.Length)
            {
                _nextPointIndex++;
                MoveToNextPointLooping(_currentLinePoints[_nextPointIndex]);
            }
        };
        
        // rotate to next movement position
        _turnTween = transform.DOLookAt(destination, tweenTime);
    }
    
    // Stop moving if an obstacle is hit
    private void OnCollisionEnter(Collision collision)
    {
        // Check if is obstacle
        if ((obstacleLayerMask & (1 << collision.collider.gameObject.layer)) != 0)
        {
            
        }
    }
}
