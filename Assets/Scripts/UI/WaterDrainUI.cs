using UnityEngine;
using UnityEngine.UI;

/*
 *  Handles the UI meter for showing how drained the water is
 *
 *  Jeff Stevenson
 *  10.5.25
 */

public class WaterDrainUI : MonoBehaviour
{
    [SerializeField] 
    private Image drainFillImage;
    
    private bool _updateWaterDrainUI;
    private WaterDrain _waterDrain;
    
    void Start()
    {
        _waterDrain = WaterDrain.Instance;
        _waterDrain.OnWaterStartDraining += () => { _updateWaterDrainUI = true; };
        _waterDrain.OnWaterDrained += () => { _updateWaterDrainUI = false; };
    }
    
    void Update()
    {
        if (_updateWaterDrainUI)
        {
            drainFillImage.fillAmount = _waterDrain.GetWaterDrainPercentage();
        }
    }
}
