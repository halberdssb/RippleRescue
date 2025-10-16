using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*
 * Keeps track of all collected items (minus level completion collectibles) for player
 * 
 * Jeff Stevenson
 * 10.9.25
 */

public class ItemInventory : MonoBehaviour
{
    public Transform keyDisplayPosition; // transform that keys are placed on/childed to when collected
    
    private List<CollectibleItem> _itemsInInventory = new List<CollectibleItem>();
    
    // Adds an item to the inventory
    public void AddItem(CollectibleItem item)
    {
        _itemsInInventory.Add(item);
    }

    // Searches inventory for item of specified type and passes it out if there is one
    public bool TryGetItemTypeFromInventory<T>(out T item, CollectibleItem.CollectiblePairingType pairingType = CollectibleItem.CollectiblePairingType.None)
    {
        foreach (CollectibleItem inventoryItem in _itemsInInventory)
        {
            // check if the inventory item is the right type
            if (inventoryItem is T)
            {
                // if there is a desired pairing type, check if the found item matches the type - otherwise always remove and return
                if (pairingType == CollectibleItem.CollectiblePairingType.None || pairingType == inventoryItem.pairingType)
                {
                    item = inventoryItem.GetComponent<T>();
                    _itemsInInventory.Remove(inventoryItem);
                    return true;
                }
            }
        }

        item = default(T);
        return false;
    }
}
