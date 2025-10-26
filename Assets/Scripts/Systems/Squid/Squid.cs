using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/*
 * Squid obstacle that slows down player on contact
 * 
 * Jeff Stevenson
 * 10.23.25
 */


public class Squid : MonoBehaviour
{
    //[HideInInspector]
    public List<Vector3> movePositions = new List<Vector3>();

    [SerializeField, Tooltip("True = squid will move from last point to first point in same direction, False = squid will move backwards through poins when it reaches end of path")]
    private bool loopThroughPoints;
    [SerializeField]
    private float moveSpeed;

    [Space] [SerializeField] private float playerSlowedSpeed;

    private int _lastMovePositionIndex;
    private int _moveDirection; // 1 is forward, -1 is backward

    private void Awake()
    {
        movePositions.Insert(0, transform.position);
    }
    void Start()
    {
        _moveDirection = 1;
        WaterDrain.Instance.OnWaterStartDraining += TweenToNextPointOnPath;
    }

    // tweens along path to next point
    private void TweenToNextPointOnPath()
    {
        Vector3 destination = movePositions[_lastMovePositionIndex + _moveDirection];
        Vector3 distanceVectorToDestination = destination - transform.position;

        // start moving to next point
        float tweenTime = distanceVectorToDestination.magnitude / moveSpeed;
        Tween moveTween = transform.DOMove(destination, tweenTime).SetEase(Ease.Linear);
        
        // look at destination
        transform.forward = distanceVectorToDestination.normalized;

        moveTween.onComplete += () =>
        {
            // check if at end of path
            if (IsAtEndOfPath())
            {
                // continue to starting position if looping
                if (loopThroughPoints)
                {
                    // set to -2 to properly move to index 0 after increment
                    _lastMovePositionIndex = -2;
                }
                // else flip move direction
                else
                {
                    // offset by two extra for correct index increment
                    _lastMovePositionIndex += 2 * _moveDirection;
                    _moveDirection *= -1;
                }
            }

            _lastMovePositionIndex += _moveDirection;
            TweenToNextPointOnPath();
        };
    }

    // checks if squid is at end of point in either direction (if not looping)
    private bool IsAtEndOfPath()
    {
        // use -2 to index because move tween uses lastMovePositionIndex+1
        return (_moveDirection == 1 && _lastMovePositionIndex >= movePositions.Count - 2||
                _moveDirection == -1 && _lastMovePositionIndex <= 1); 
    }
    
    // collide with player and slow them down
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LineFollower playerLineFollower = other.GetComponent<LineFollower>();
            playerLineFollower.SetLineFollowSpeed(playerSlowedSpeed);
            gameObject.SetActive(false);
        }
    }

    // draw path between all movement points
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        // draw first line to player
        Gizmos.DrawLine(transform.position, movePositions[0]);
        
        // draw last line to player if looping
        if (loopThroughPoints)
        {
            Gizmos.DrawLine(movePositions[movePositions.Count - 1], transform.position);

        }

        // draw lines betweens all other points
        for (int i = 1; i < movePositions.Count; i++)
        {
            Vector3 startPos = movePositions[i - 1];
            Vector3 endPos = movePositions[i];

            Gizmos.DrawLine(startPos, endPos);
        }
    }
}
