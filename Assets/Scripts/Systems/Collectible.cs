using UnityEngine;

/*
 *  Object the player can collect during puzzles
 *
 *  Jeff Stevenson 
 *  9.26.25
 */

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class Collectible : MonoBehaviour
{
    public delegate void CollectedDelegate();
    public CollectedDelegate OnCollected;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Collect game object if player collides
        if (other.gameObject.CompareTag("Player"))
        {
            OnCollected?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
