using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float sideSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Constant forward movement
        float moveX = forwardSpeed * Time.deltaTime;

        // Side movement
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveZ = sideSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveZ = -sideSpeed * Time.deltaTime;
        }

        transform.Translate(moveX, 0f, moveZ, Space.World);

        // Jump when pressing space
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detect ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}