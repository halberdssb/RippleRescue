using DG.Tweening;
using UnityEngine;

/*
 * Missile object that launches in the faced direction when the player colliders with it
 * 
 * Jeff Stevenson
 * 10.16.25
 */

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Missile : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Determines if the missile will start rotating when water starts draining or if it will stay pointing in the placed direction.")]
    private bool rotateOnWaterDrain;
    [SerializeField]
    private float rotationSpeed;

    [Space, SerializeField]
    private float launchForce;

    private bool _hasBeenLaunched;
    private bool _rotating;
    private Rigidbody _rb;
    private Collider _collider;

    [Space]
    [SerializeField]
    private GameObject art;

    private Tween spinTween;
    private float spinTime = 2f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        if (rotateOnWaterDrain)
        {
            WaterDrain.Instance.OnWaterStartDraining += () => StartSpinTween();
            
        }
    }

    private void Update()
    {

    }

    // fires missile in facing direction
    private void LaunchMissile()
    {
        if (rotateOnWaterDrain)
        {
            _rotating = false;
        }

        _hasBeenLaunched = true;
        Vector3 launchVector = launchForce * transform.forward;
        _rb.AddForce(launchVector, ForceMode.Impulse);
    }

    // turns off art and collision for missile
    private void DisableMissile()
    {
        art.SetActive(false);
        _collider.enabled = false;
    }

    // start movement by player if player collides
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LaunchMissile();
        }
    }

    // destroy missile on collision with collider
    private void OnCollisionEnter(Collision collision)
    {
        if (_hasBeenLaunched && collision.gameObject.CompareTag("Obstacle"))
        {
            DisableMissile();
        }
    }

    private void StartSpinTween()
    {
        Vector3 fullSpinRotation = transform.eulerAngles;
        fullSpinRotation.y += 360;
        
        spinTween = transform.DOLocalRotate(fullSpinRotation, spinTime);
        spinTween.onComplete += StartSpinTween;
    }
}
