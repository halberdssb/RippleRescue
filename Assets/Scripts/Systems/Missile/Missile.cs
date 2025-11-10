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

    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask destroyMissileMask;

    [Space, SerializeField]
    private float launchForce;

    private bool _hasBeenLaunched;
    private bool _rotating;
    private Rigidbody _rb;
    private Collider _collider;

    [Space]
    [SerializeField]
    private GameObject art;
    [SerializeField]
    private Animator animator;

    [Space] 
    [SerializeField] 
    private AudioSource launchSound;
    [SerializeField] 
    private AudioSource destroySound;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        if (rotateOnWaterDrain)
        {
            WaterDrain.Instance.OnWaterStartDraining += () => _rotating = true;
            WaterDrain.Instance.OnWaterDrained += () =>
            {
                animator.SetBool("Launch", false);
                _rotating = false;
            };
        }
    }

    private void Update()
    {
        if (_rotating)
        {
            transform.eulerAngles += new Vector3(0f, rotationSpeed * Time.deltaTime, 0f);
        }
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
        
        animator.SetBool("Launch", true);
        launchSound.Play();
    }

    // turns off art and collision for missile
    private void DisableMissile()
    {
        _rb.linearVelocity = Vector3.zero;
        _collider.enabled = false;
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce).onComplete += () =>
        {
            art.SetActive(false);
        };
    }

    // start movement by player if player collides
    private void OnTriggerEnter(Collider other)
    {
        // launch from player
        if (other.CompareTag("Player"))
        {
            LaunchMissile();
        }
        
        // hit obstacle
        if (_hasBeenLaunched && (obstacleMask & (1 << other.gameObject.layer)) != 0)
        {
            // break object if obstacle
            if ((destroyMissileMask & (1 << other.gameObject.layer)) != 0)
            {
                other.gameObject.SetActive(false);
                destroySound.Play();
            }
            DisableMissile();
        }
    }
}
