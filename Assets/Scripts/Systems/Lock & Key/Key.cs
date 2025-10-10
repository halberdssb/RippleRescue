using UnityEngine;

/*
 * Key object that unlocks Lock objects on collision when collected
 * 
 * Jeff Stevenson
 * 10.9.25
 */

public class Key : CollectibleItem
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // When collected, add to inventory and move to position on player model
    protected override void CollectItem(ItemInventory playerInventory)
    {
        base.CollectItem(playerInventory);
        
        // move key to position on player
        transform.SetParent(playerInventory.keyDisplayPosition);
        transform.position = playerInventory.keyDisplayPosition.position;
        transform.rotation = playerInventory.keyDisplayPosition.rotation;
    }
}
