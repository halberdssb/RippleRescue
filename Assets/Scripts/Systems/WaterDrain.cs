using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
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

    [Space] [SerializeField] private GameObject drainVFX;
    
    private bool _waterDraining;
    private Tween _waterTween;
    private float _startDrainYPosition;
    private float _endDrainYPosition;
    private float _fillUpTweenTime = 3f;

    private float _bubbleFillUpTweenTime = 1.5f;
    

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
        drainVFX.SetActive(false);
        OnWaterStartDraining += () => drainVFX.SetActive(true);
        
        _startDrainYPosition = transform.position.y;
        _endDrainYPosition = waterPlaneEndPosition.position.y;
        

        InstantDrainWater();
    }

    // Starts the water drain tween
    public void StartWaterDrain(float tweenTime)
    {
        // start drain and fire start delegate
        if (!_waterDraining)
        {
            OnWaterStartDraining?.Invoke();
        }
        
        _waterDraining = true;

        // tween down to end position
        _waterTween = transform.DOMoveY(_endDrainYPosition, tweenTime);

        // on complete, set bool false and fire water drained delegate
        _waterTween.SetEase(Ease.Linear);
        _waterTween.onComplete = () =>
        {
            _waterDraining = false;
            OnWaterDrained?.Invoke();
        };
    }

    public void StartWaterDrain()
    {
        StartWaterDrain(DrainTime);
    }

    // returns the 0-1 value of the water position as it drains (1 is full, 0 is drained)
    public float GetWaterDrainPercentage()
    {
        return Mathf.InverseLerp(_endDrainYPosition, _startDrainYPosition, transform.position.y);
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

    // fills up the bathtub on level start
    public void FillUpBathtub(Action onFillComplete)
    {
        transform.DOMoveY(_startDrainYPosition, _fillUpTweenTime).onComplete += () => onFillComplete?.Invoke();
    }

    public void MoveWaterUpByAmount(float movementY)
    {
        _waterTween.Kill();
        
        // minus movement because of inverted bathtub up movement
        float endPositionY = transform.position.y - movementY;
        float drainTimeLeft = Mathf.InverseLerp(_endDrainYPosition, _startDrainYPosition, endPositionY) * DrainTime;
        Tween bubbleTween = transform.DOMoveY(endPositionY, _bubbleFillUpTweenTime).SetEase(Ease.Linear);
        bubbleTween.onComplete += () =>
        {
            Debug.Log("bubble fill finished");
            StartWaterDrain(drainTimeLeft);
        };
    }
}
