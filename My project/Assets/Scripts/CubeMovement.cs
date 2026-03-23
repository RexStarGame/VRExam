using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] public float forwardSpeed = 5f;
    [SerializeField] public float sideSpeed = 5f;

    [Header("RigidBody Physics")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] public float jumpVelocity = 12f;
    [SerializeField] public float gravityMultiplier = 4f;
    [SerializeField] public float flipSpeed = 360f;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] bool otherGravity = false;

    [Header("Portal")]
    [SerializeField] private bool gravityReversed;

    [Header("Audio")]
    // TILFÃ˜JET 1: En variabel til at holde fast i dit lyd-script
    [SerializeField] private JumpAudioManager audioManager;

    [Header("Event")]
    [SerializeField] private PlayerEvents playerEvents;

    void Start()
    {
        if (otherGravity) Physics.gravity = new(0,-gravityMultiplier,0);
        InitializeEventListeners();
        InitializeRigidBody();
        InitializeAudio();
        InitializeCollider();
    }

    void FixedUpdate()
    {
        // Forward movement
        Vector3 newPosition = rb.position + Vector3.right * forwardSpeed * Time.fixedDeltaTime;

        // Side movement
        if (Input.GetKey(KeyCode.A))
            newPosition += sideSpeed * Time.fixedDeltaTime * Vector3.forward;

        if (Input.GetKey(KeyCode.D))
            newPosition += sideSpeed * Time.fixedDeltaTime * Vector3.back;

        rb.MovePosition(newPosition);

        // Gravity while in reverse gravity
        if (gravityReversed == true)
        {
            rb.linearVelocity -= Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
        }
        if (!otherGravity)
        {
            // Faster falling
            if (rb.linearVelocity.y < 0 && gravityReversed == false)
            {
                rb.linearVelocity += (gravityMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
            }
            // and in reverse gravity
            else if (rb.linearVelocity.y > 0 && gravityReversed == true)
            {
                rb.linearVelocity -= (gravityMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
            }
        }
        // Rotate in air
        if (!isGrounded)
        {
            transform.Rotate(flipSpeed * Time.fixedDeltaTime * -Vector3.forward, Space.Self);
        }
    }

    void Update()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;


            // Allow Z rotation while airborne
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            rb.angularVelocity = Vector3.zero;

            if (gravityReversed == false) { rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z); }
            else { rb.linearVelocity = new Vector3(rb.linearVelocity.x, -jumpVelocity, rb.linearVelocity.z); }
            if (audioManager != null)
            {
                audioManager.PlayJumpSound();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isGrounded && gravityReversed == false)
        {
            isGrounded = true;

            // Stop all physics spin
            rb.angularVelocity = Vector3.zero;

            // Stop vertical bounce
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Snap to nearest 90°
            Vector3 rot = transform.eulerAngles;
            rot.z = Mathf.Round(rot.z / 90f) * 90f;
            rot.x = 0f;
            rot.y = 0f;
            transform.eulerAngles = rot;

            // Lock cube to ground instantly
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else if (collision.gameObject.CompareTag("Roof") && !isGrounded && gravityReversed == true)
        {
            isGrounded = true;

            // Stop all physics spin
            rb.angularVelocity = Vector3.zero;

            // Stop vertical bounce
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Snap to nearest 90°
            Vector3 rot = transform.eulerAngles;
            rot.z = Mathf.Round(rot.z / 90f) * 90f;
            rot.x = 0f;
            rot.y = 0f;
            transform.eulerAngles = rot;

            // Lock cube to ground instantly
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    // The part Leyla formatted but is originally written by William
    private void InitializeRigidBody()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    private void InitializeAudio()
    {
        audioManager = GetComponent<JumpAudioManager>();
    }
    private void InitializeCollider()
    {
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            PhysicsMaterial mat = new()
            {
                bounciness = 0f,
                frictionCombine = PhysicsMaterialCombine.Multiply,
                bounceCombine = PhysicsMaterialCombine.Multiply
            };
            collider.material = mat;
        }
        else
        {
            collider = gameObject.AddComponent<BoxCollider>();
            PhysicsMaterial mat = new()
            {
                bounciness = 0f,
                frictionCombine = PhysicsMaterialCombine.Multiply,
                bounceCombine = PhysicsMaterialCombine.Multiply
            };
            collider.material = mat;
        }
    }
    // Leyla's Part
    private void InitializeEventListeners()
    {
        playerEvents = PlayerEvents.instance;
        playerEvents.GravityEvent.AddListener(OnGravity);
    }
    private void OnGravity()
    {
        gravityReversed = !gravityReversed;
        rb.useGravity = !gravityReversed;
    }
}