using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] canvasGroups;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaterDrain.Instance.OnWaterStartDraining += () => SetPlayerHUDInteractivity(false);
    }

   // sets interactivity for all canvas groups in the player hud 
   private void SetPlayerHUDInteractivity(bool interactable)
   {
       Debug.Log("interactable");
       foreach (var canvasGroup in canvasGroups)
       {
           canvasGroup.interactable = interactable;
       }
   }
}
