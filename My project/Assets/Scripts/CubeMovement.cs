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
    [SerializeField] private JumpAudioManager audioManager;

    [Header("Event")]
    [SerializeField] private PlayerEvents playerEvents;

    [Header("Ground Detection (NY)")]
    [Tooltip("Husk at oprette et 'Ground' Layer i Unity og sæt dine gulve/lofter til dette!")]
    [SerializeField] private LayerMask groundLayer;
    //[SerializeField] private float castDistance = 0.55f;

    private bool wasGrounded;

    // NYT: En timer der forhindrer os i at lande i det millisekund vi hopper
    private float jumpCooldownTimer = 0f;

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
        // Tjekker hele tiden om vi er på jorden
        CheckGrounded();

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
                rb.linearVelocity += (gravityMultiplier - 2) * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
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
        // NYT: Tæl vores jump cooldown ned
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        // Jump (Vi tjekker nu også, at vores cooldown er slut)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpCooldownTimer <= 0f)
        {
            isGrounded = false;

            // Sæt timeren til 0.15 sekunder, så vi får lov til at lette
            jumpCooldownTimer = 0.15f;

            Debug.Log("<color=cyan>[DEBUG]</color> HOPPER! isGrounded blev sat til false manuelt.");

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

    private void CheckGrounded()
    {
        // NYT: Hvis vi lige er hoppet, skal vi ignorere jorden!
        if (jumpCooldownTimer > 0f)
        {
            return;
        }

        wasGrounded = isGrounded;

        Vector3 direction = gravityReversed ? Vector3.up : Vector3.down;
        Vector3 checkPosition = transform.position + (direction * 0.5f);
        Vector3 boxHalfExtents = new Vector3(0.45f, 0.2f, 0.45f);

        Collider[] hitColliders = Physics.OverlapBox(checkPosition, boxHalfExtents, transform.rotation, groundLayer);

        isGrounded = hitColliders.Length > 0;

        // Landing logic
        if (isGrounded && !wasGrounded)
        {
            Debug.Log($"<color=green>[DEBUG]</color> JORD REGISTRERET! Terningen landede på: <b>{hitColliders[0].gameObject.name}</b>");
            HandleLanding();
        }
        else if (!isGrounded && wasGrounded)
        {
            Debug.Log("<color=orange>[DEBUG]</color> I LUFTEN! Terningen forlod jorden.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;

        Vector3 direction = gravityReversed ? Vector3.up : Vector3.down;
        Vector3 checkPosition = transform.position + (direction * 0.5f);
        Vector3 boxHalfExtents = new Vector3(0.45f, 0.2f, 0.45f);

        Gizmos.matrix = Matrix4x4.TRS(checkPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);
    }

    private void HandleLanding()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 rot = transform.eulerAngles;
        rot.z = Mathf.Round(rot.z / 90f) * 90f;
        rot.x = 0f;
        rot.y = 0f;
        transform.eulerAngles = rot;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void InitializeRigidBody()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        // TILFØJET LØSNING PÅ TUNNELING:
        // Tvinger Unity til at beregne kollisioner præcist uden at "glida igennem"
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
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

    private void InitializeEventListeners()
    {
        playerEvents = PlayerEvents.instance;
        if (playerEvents != null)
        {
            playerEvents.GravityEvent.AddListener(OnGravity);
        }
    }

    private void OnGravity()
    {
        gravityReversed = !gravityReversed;
        rb.useGravity = !gravityReversed;
    }
}