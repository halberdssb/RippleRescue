using DG.Tweening;
using UnityEngine;

/*
 * Lock that blocks player movement until collided with when player has Key
 * 
 * Jeff Stevenson
 * 10.9.25
 */

[RequireComponent(typeof(AudioSource))]
public class Lock : MonoBehaviour, ICollisionEvent
{
    private BoxCollider collider;
    private float unlockTweenTime = 1f;
    private float unlockTweenYOffset = -1f;
    
    private AudioSource unlockSound;

    [SerializeField]
    private CollectibleItem.CollectiblePairingType keyPairingType;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        unlockSound = GetComponent<AudioSource>();
    }
    // On collision with player, try look for key and unlock/turn off lock if they have one
    public void OnCollisionEvent(GameObject player, out bool stopMovement)
    {
        // assume player doesn't have key until checking
        stopMovement = true;
        
        // Get player inventory
        if (player.TryGetComponent<ItemInventory>(out var playerInventory))
        {
            // If player has key, turn off key and lock and keep player moving
            if (playerInventory.TryGetItemTypeFromInventory(out Key key, keyPairingType))
            {
                key.gameObject.SetActive(false);
                
                // turn off lock and tween away
                DisableAndHideLock();
                // play unlock sound
                unlockSound.Play();
            
                stopMovement = false;   
            }
        }
        // Error if player does not have inventory component
        else
        {
            Debug.LogError("Player does not have inventory component accessible to " + name + " for collision event.", this);
        }
    }
    
    // Disables lock collision and tweens it below water to hide it when unlocked
    private void DisableAndHideLock()
    {
        collider.enabled = false;
        Vector3 moveTweenPosition = transform.position;
        transform.DOMoveY(moveTweenPosition.y + unlockTweenYOffset, unlockTweenTime);
    }
}
