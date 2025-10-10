using UnityEngine;

/*
 *  Interface for obstacles that should fire an event on collision with the player
 *
 * 
 */

public interface ICollisionEvent
{
    public void OnCollisionEvent(GameObject player, out bool stopMovement);
}
