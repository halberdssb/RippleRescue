using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float addedFillValue;

    // fill up water some when added
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WaterDrain.Instance.MoveWaterUpByAmount(addedFillValue);
            gameObject.SetActive(false);
        }
    }
}
