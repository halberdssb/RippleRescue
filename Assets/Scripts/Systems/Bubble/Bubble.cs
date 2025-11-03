using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float addedFillValue;
    [SerializeField] private GameObject art;
    [SerializeField] private AudioSource audioSource;

    private Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();    
    }
    
    // fill up water some when added
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WaterDrain.Instance.MoveWaterUpByAmount(addedFillValue);
            art.SetActive(false);
            collider.enabled = false;
            audioSource.Play();
        }
    }
}
