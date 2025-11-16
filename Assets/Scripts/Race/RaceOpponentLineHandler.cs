using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif

/*
 *  Handles creating the line for race opponents to follow in race mode
 *
 *  Jeff Stevenson
 *  11.16.25
 */

public class RaceOpponentLineHandler : MonoBehaviour
{
    [SerializeField] 
    private RaceOpponentPathData pathData;
    
    [Space]
    [SerializeField]
    private bool enableLineDrawer;
    
    private LineFollower _lineFollower;
    
    void Start()
    {
        // get line components
        _lineFollower = GetComponent<LineFollower>();

        transform.position = pathData.linePoints[0];
        _lineFollower.MoveAlongLineToEnd(pathData.linePoints);
    }
}
