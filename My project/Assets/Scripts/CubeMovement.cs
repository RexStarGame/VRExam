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

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze X and Y rotation for flips along Z
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Disable Rigidbody bounciness
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            PhysicsMaterial mat = new PhysicsMaterial(); // updated from PhysicMaterial
            mat.bounciness = 0f;
            mat.frictionCombine = PhysicsMaterialCombine.Multiply; // updated
            mat.bounceCombine = PhysicsMaterialCombine.Multiply;   // updated
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

        // Apply snappy falling only if moving down
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
        }

        // Rotate cube along -Z while in the air
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
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isGrounded)
        {
            isGrounded = true;

            // Zero vertical velocity to prevent bounce
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Snap rotation to nearest 90 degrees on Z
            Vector3 rot = transform.eulerAngles;
            rot.z = Mathf.Round(rot.z / 90f) * 90f;
            rot.x = 0f;
            rot.y = 0f;
            transform.eulerAngles = rot;
        }
    }
}