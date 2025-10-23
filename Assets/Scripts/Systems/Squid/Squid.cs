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
    [HideInInspector]
    public List<Vector3> movePositions = new List<Vector3>();

    [SerializeField, Tooltip("True = squid will move from last point to first point in same direction, False = squid will move backwards through poins when it reaches end of path")]
    private bool loopThroughPoints;

    private void Awake()
    {
        movePositions.Insert(0, transform.position);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
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
