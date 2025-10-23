using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/*
 * Handles water draining within puzzle over time
 * 
 * Jeff Stevenson
 * 10.2.25
 */

public class WaterDrain : MonoBehaviour
{
    public static WaterDrain Instance;

    [Tooltip("Total time until the water drains.")]
    public float DrainTime = 5;

    public delegate void WaterStartDrainingDelegate();
    public WaterStartDrainingDelegate OnWaterStartDraining;

    public delegate void WaterDrainedDelegate();
    public WaterDrainedDelegate OnWaterDrained;

    [SerializeField]
    [Tooltip("The end position that the water plane will tween down to - only Y position is used.")]
    private Transform waterPlaneEndPosition;

    private bool _waterDraining;
    private Tween _waterTween;
    private float _startWaterYPosition;
    private float _endDrainYPosition;


    void Awake()
    {
        if (Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }
    void Start()
    {
        _startWaterYPosition = transform.position.y;
        _endDrainYPosition = waterPlaneEndPosition.position.y;

        InstantDrainWater();
    }

    // Starts the water drain tween
    public void StartWaterDrain()
    {
        // start drain and fire start delegate
        _waterDraining = true;
        OnWaterStartDraining?.Invoke();

        // tween down to end position
        _waterTween = transform.DOMoveY(_endDrainYPosition, DrainTime);

        // on complete, set bool false and fire water drained delegate
        _waterTween.SetEase(Ease.Linear);
        _waterTween.onComplete = () =>
        {
            _waterDraining = false;
            OnWaterDrained?.Invoke();
        };
    }

    // returns the 0-1 value of the water position as it drains (1 is full, 0 is drained)
    public float GetWaterDrainPercentage()
    {
        return Mathf.InverseLerp(_endDrainYPosition, _startWaterYPosition, transform.position.y);
    }

    // Updates the drain end position to the current position of the plane
    [ContextMenu("Set End Drain Position To Current Position")]
    private void SetEndDrainPositionToCurrentPosition()
    {
        waterPlaneEndPosition.position = transform.position;
    }

    // sets the tub to be fully drained/empty - used on scene start for fill up sequence
    private void InstantDrainWater()
    {
        transform.position = new Vector3(transform.position.x, waterPlaneEndPosition.position.y, transform.position.z);
    }
}
