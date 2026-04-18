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

    [Header("Portal & State")]
    [SerializeField] private bool gravityReversed;

    // NYT: Buffer variabler
    private float jumpBufferCounter;
    private float jumpBufferTime = 0.15f;
    private float jumpCooldownTimer = 0f;

    [Header("Audio & Events")]
    [SerializeField] private JumpAudioManager audioManager;
    [SerializeField] private PlayerEvents playerEvents;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;

    private bool wasGrounded;

    void Start()
    {
        if (otherGravity) Physics.gravity = new(0, -gravityMultiplier, 0);
        InitializeEventListeners();
        InitializeRigidBody();
        InitializeAudio();
        InitializeCollider();
    }

    void FixedUpdate()
    {
        CheckGrounded();

        // Fremadgående bevægelse
        Vector3 newPosition = rb.position + Vector3.right * forwardSpeed * Time.fixedDeltaTime;

        // Side bevægelse (A/D)
        if (Input.GetKey(KeyCode.A))
            newPosition += sideSpeed * Time.fixedDeltaTime * Vector3.forward;
        if (Input.GetKey(KeyCode.D))
            newPosition += sideSpeed * Time.fixedDeltaTime * Vector3.back;

        rb.MovePosition(newPosition);

        // Tyngdekraft logik
        ApplyCustomGravity();

        // Rotation i luften
        if (!isGrounded)
        {
            transform.Rotate(flipSpeed * Time.fixedDeltaTime * -Vector3.forward, Space.Self);
        }
    }

    void Update()
    {
        // Tæl timere ned
        if (jumpCooldownTimer > 0f) jumpCooldownTimer -= Time.deltaTime;

        // JUMP BUFFER: Hvis vi trykker Space, starter vi en timer
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // AUTO-JUMP: Hvis man holder Space nede, skal bufferen blive ved med at være aktiv
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        // Tjek om vi må hoppe (Vi har et tryk i bufferen + vi er på jorden + ingen cooldown)
        if (jumpBufferCounter > 0f && isGrounded && jumpCooldownTimer <= 0f)
        {
            ExecuteJump();
        }
    }

    private void ExecuteJump()
    {
        isGrounded = false;
        jumpBufferCounter = 0f; // Nulstil bufferen så vi ikke dobbelt-hopper
        jumpCooldownTimer = 0.1f; // Kort cooldown så vi kan nå at slippe jorden

        // Lås rotation mens vi hopper
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        rb.angularVelocity = Vector3.zero;

        // Beregn jump retning baseret på tyngdekraft
        float v = gravityReversed ? -jumpVelocity : jumpVelocity;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, v, rb.linearVelocity.z);

        if (audioManager != null) audioManager.PlayJumpSound();
    }

    private void CheckGrounded()
    {
        wasGrounded = isGrounded;

        Vector3 direction = gravityReversed ? Vector3.up : Vector3.down;
        Vector3 checkPosition = transform.position + (direction * 0.5f);

        // Vi bruger en lidt bredere boks (0.48f) for at undgå at falde igennem kanter
        Vector3 boxHalfExtents = new Vector3(0.48f, 0.1f, 0.48f);

        Collider[] hitColliders = Physics.OverlapBox(checkPosition, boxHalfExtents, transform.rotation, groundLayer);

        // Hvis vi lige er hoppet, ignorerer vi jorden et kort øjeblik
        if (jumpCooldownTimer > 0f)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = hitColliders.Length > 0;
        }

        // Landing logik
        if (isGrounded && !wasGrounded)
        {
            HandleLanding();
        }
    }

    private void HandleLanding()
    {
        // Stop vertikal bevægelse med det samme for at undgå jitter
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Snap rotation til nærmeste 90 grader
        Vector3 rot = transform.eulerAngles;
        rot.z = Mathf.Round(rot.z / 90f) * 90f;
        rot.x = 0f;
        rot.y = 0f;
        transform.eulerAngles = rot;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void ApplyCustomGravity()
    {
        if (gravityReversed)
        {
            rb.linearVelocity -= Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
        }

        if (!otherGravity)
        {
            if (rb.linearVelocity.y < 0 && !gravityReversed)
            {
                rb.linearVelocity += (gravityMultiplier - 2) * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
            }
            else if (rb.linearVelocity.y > 0 && gravityReversed)
            {
                rb.linearVelocity -= (gravityMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
            }
        }
    }

    // --- INITIALIZATION METODER (Uændrede) ---
    private void InitializeRigidBody()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    private void InitializeAudio() => audioManager = GetComponent<JumpAudioManager>();

    private void InitializeCollider()
    {
        var col = GetComponent<Collider>();
        PhysicsMaterial mat = new() { bounciness = 0f, frictionCombine = PhysicsMaterialCombine.Multiply };
        if (col != null) col.material = mat;
    }

    private void InitializeEventListeners()
    {
        playerEvents = PlayerEvents.instance;
        if (playerEvents != null) playerEvents.GravityEvent.AddListener(OnGravity);
    }

    private void OnGravity()
    {
        gravityReversed = !gravityReversed;
        rb.useGravity = !gravityReversed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 direction = gravityReversed ? Vector3.up : Vector3.down;
        Vector3 checkPosition = transform.position + (direction * 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(checkPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.48f * 2, 0.2f, 0.48f * 2));
    }
}