using UnityEngine;

/*
 *  Object the player collects for level rank/completion
 *
 *  Jeff Stevenson 
 *  9.26.25
 */

public class LevelCompletionCollectible : CollectibleItem
{
    public delegate void CollectedDelegate();
    public CollectedDelegate OnCollected;

    // Collect gameobject, turn off, and fire collected delegate when collected
    protected override void CollectItem(ItemInventory playerInventory)
    {
        base.CollectItem(playerInventory);
        
        OnCollected?.Invoke();
        gameObject.SetActive(false);
    }
}
