using UnityEngine;

/*
 *  Object the player collects for level rank/completion
 *
 *  Jeff Stevenson 
 *  9.26.25
 */

[RequireComponent(typeof(AudioSource))]
public class LevelCompletionCollectible : CollectibleItem
{
    public delegate void CollectedDelegate();
    public CollectedDelegate OnCollected;

    private AudioSource collectedSound;
    private MeshRenderer mesh;
    private Collider collider;

    private void Awake()
    {
        collectedSound = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }
    
    // Collect gameobject, turn off, and fire collected delegate when collected
    protected override void CollectItem(ItemInventory playerInventory)
    {
        base.CollectItem(playerInventory);
        
        OnCollected?.Invoke();
        mesh.enabled = false;
        collider.enabled = false;
        collectedSound.Play();
    }
}
