using UnityEngine;

/*
 * Lock that blocks player movement until collided with when player has Key
 * 
 * Jeff Stevenson
 * 10.9.25
 */

public class Lock : MonoBehaviour, ICollisionEvent
{
    // On collision with player, try look for key and unlock/turn off lock if they have one
    public void OnCollisionEvent(GameObject player, out bool stopMovement)
    {
        // assume player doesn't have key until checking
        stopMovement = true;
        
        // Get player inventory
        if (player.TryGetComponent<ItemInventory>(out var playerInventory))
        {
            // If player has key, turn off key and lock and keep player moving
            if (playerInventory.TryGetItemTypeFromInventory(out Key key))
            {
                key.gameObject.SetActive(false);
                gameObject.SetActive(false);
            
                stopMovement = false;   
            }
        }
        // Error if player does not have inventory component
        else
        {
            Debug.LogError("Player does not have inventory component accessible to " + name + " for collision event.", this);
        }
    }
}
