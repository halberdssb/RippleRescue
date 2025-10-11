using UnityEngine;

/*
 * Base collectible item class that can be picked up by player and added to inventory
 * 
 * Jeff Stevenson
 * 10.10.25
 */

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class CollectibleItem : MonoBehaviour
{
    // overrideable collection function - default just adds to player inventory
    protected virtual void CollectItem(ItemInventory playerInventory)
    {
        // Add item
        playerInventory.AddItem(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Collect game object if player collides
        if (other.gameObject.CompareTag("Player"))
        {
            // Get player inventory
            if (other.gameObject.TryGetComponent<ItemInventory>(out var playerInventory))
            {
                // Add item
                playerInventory.AddItem(this);

                CollectItem(playerInventory);
            }
            // Error if playe rdoes not have inventory
            else
            {
                Debug.LogError("Player does not have inventory while trying to collect collectible: " + this, this);
            }
        }
    }
}
