using UnityEngine;
using System.Collections.Generic;

/*
 * Keeps track of all collected items (minus level completion collectibles) for player
 * 
 * Jeff Stevenson
 * 10.9.25
 */

public class ItemInventory : MonoBehaviour
{
    public List<CollectibleItem> itemsInInventory = new List<CollectibleItem>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
