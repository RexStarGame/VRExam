using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float sideSpeed = 5f;
    public float jumpVelocity = 12f;
    public float gravityMultiplier = 4f;
    public float flipSpeed = 360f;

    private Rigidbody rb;
    private bool isGrounded = true;

    // TILFÃ˜JET 1: En variabel til at holde fast i dit lyd-script
    private JumpAudioManager audioManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isGrounded = true;
        audioManager = GetComponent<JumpAudioManager>();


        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Start locked to ground
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Remove bounce

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        rb.interpolation = RigidbodyInterpolation.Interpolate;


        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            PhysicsMaterial mat = new PhysicsMaterial();
            mat.bounciness = 0f;
            mat.frictionCombine = PhysicsMaterialCombine.Multiply;
            mat.bounceCombine = PhysicsMaterialCombine.Multiply;
            collider.material = mat;
        }
    }

    void FixedUpdate()
    {
        // Forward movement
        Vector3 newPosition = rb.position + Vector3.right * forwardSpeed * Time.fixedDeltaTime;

        // Side movement
        if (Input.GetKey(KeyCode.A))
            newPosition += Vector3.forward * sideSpeed * Time.fixedDeltaTime;

        if (Input.GetKey(KeyCode.D))
            newPosition += Vector3.back * sideSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        // Faster falling
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
        }

        // Rotate in air
        if (!isGrounded)
        {
            transform.Rotate(-Vector3.forward * flipSpeed * Time.fixedDeltaTime, Space.Self);
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

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);

            if (audioManager != null)
            {
                audioManager.PlayJumpSound();
            }

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isGrounded)
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
}