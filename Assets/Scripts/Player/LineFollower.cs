using System.Collections.Generic;
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
    public float MoveSpeed;

    [SerializeField] 
    private LayerMask obstacleLayerMask;

    [SerializeField] private AudioSource collisionStopSound;

    public delegate void FinishedFollowingLineDelegate();
    public FinishedFollowingLineDelegate OnFinishedFollowingLine;
    
    private LineDrawer _lineDrawer;
    
    private Tween _moveTween;
    private Tween _turnTween;
    private Vector3[] _currentLinePoints;
    private int _nextPointIndex;

    private List<RaceCheckpoint> _hitCheckpoints = new List<RaceCheckpoint>();
    private int _numLapsCompleted;

    private void Start()
    {
        _lineDrawer = GetComponent<LineDrawer>();

        // subscribe movement to water drain start
        if (GameManager.Instance.gameMode == GameManager.GameMode.Puzzle)
        {
            WaterDrain.Instance.OnWaterStartDraining += PlayerMoveAlongLine;
            WaterDrain.Instance.OnWaterDrained -= () => WaterDrain.Instance.OnWaterStartDraining -= PlayerMoveAlongLine;
            WaterDrain.Instance.OnWaterDrained += StopFollowingLine;
        }
        else
        {
            if (_lineDrawer != null)
            {
                OnFinishedFollowingLine += () => _lineDrawer.ResetLine();
            }
        }
    }

    public void PlayerMoveAlongLine()
    {
        if (_lineDrawer.LinePoints.Length <= 0) return;
        
        MoveAlongLineToEnd(_lineDrawer.LinePoints);
    }
    
    // Begins set movement along a line through all points
    public void MoveAlongLineToEnd(Vector3[] inLinePoints)
    {
        // get line points from line drawer
        _currentLinePoints = inLinePoints;
        
        // start moving along points
        _nextPointIndex = 1;
        MoveToNextPointLooping(_currentLinePoints[_nextPointIndex]);
    }
    
    // Sets the current points of the line
    public void SetCurrentLinePoints(Vector3[] linePoints)
    {
        _currentLinePoints = linePoints;
    }

    // Starts a tween to move toward the next point at a constant speed and cues movement to next point on tween complete
    private void MoveToNextPointLooping(Vector3 destination)
    {
        Vector3 distanceVectorToDestination = destination - transform.position;
        
        // start moving to next point
        float tweenTime = distanceVectorToDestination.magnitude / MoveSpeed;
        _moveTween = transform.DOMove(destination, tweenTime);
        _moveTween.SetEase(Ease.Linear);
        
        // cue move to next point if there is one on tween complete
        if (_nextPointIndex + 1 < _currentLinePoints.Length)
        {
            _moveTween.onComplete += () =>
            {
                // remove previous point from drawn line and update visuals
                if (_lineDrawer != null)
                {
                    _lineDrawer.RemoveLinePointAtIndex(0);
                }

                // begin movement to next point on line
                _nextPointIndex++;
                MoveToNextPointLooping(_currentLinePoints[_nextPointIndex]);
            };
        }
        // otherwise line is done moving - check game state
        else
        {
            OnFinishedFollowingLine?.Invoke();
        }
        
        // rotate to next movement position
        _turnTween = transform.DOLookAt(destination, tweenTime);
    }
    
    // Stop moving if an obstacle is hit
    private void OnCollisionEnter(Collision collision)
    {
        // Check if is obstacle
        if ((obstacleLayerMask & (1 << collision.collider.gameObject.layer)) != 0)
        {
            bool stopMovement = true;
            
            // fire collision event if it exists
            if (collision.collider.gameObject.TryGetComponent(out ICollisionEvent collisionEvent))
            {
                collisionEvent.OnCollisionEvent(gameObject, out stopMovement);
            }
            
            // stop movement
            if (stopMovement)
            {
                if (collisionStopSound != null)
                {
                    collisionStopSound.Play();
                }
                StopFollowingLine();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
        // check for race checkpoint
        if (other.gameObject.TryGetComponent(out RaceCheckpoint checkpoint))
        {
            Debug.Log("Checkpoint hit!");
            if (!_hitCheckpoints.Contains(checkpoint))
            {
                Debug.Log("Checkpoint added!");
                _hitCheckpoints.Add(checkpoint);
            }
        }
        // check for race finish line
        else if (other.gameObject.TryGetComponent(out RaceFinishLine finishLine))
        {
            GameManager.Instance.OnLapCompleted(this);
        }
    }
    
    // Stops all movement
    private void StopFollowingLine()
    {
        _moveTween.Kill();
        _turnTween.Kill();
        OnFinishedFollowingLine?.Invoke();
    }
    
    // Sets player speed
    public void SetLineFollowSpeed(float speed)
    {
        MoveSpeed = speed;
    }
    
    // Gets number of hit checkpoints
    public int GetNumberOfHitCheckpoints()
    {
        return _hitCheckpoints.Count;
    }
    
    // Resets hit checkpoints
    public void ResetHitCheckpointsList()
    {
        _hitCheckpoints.Clear();
    }
    
    // increments laps completed
    public void CompleteLap()
    {
        _numLapsCompleted++;
        ResetHitCheckpointsList();
    }
    
    // returns number of completed laps
    public int GetNumberOfCompletedLaps()
    {
        return _numLapsCompleted;
    }
}
