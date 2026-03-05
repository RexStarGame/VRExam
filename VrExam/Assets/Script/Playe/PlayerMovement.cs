using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class VRControllerLocomotion : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform head;  
    [SerializeField] private CharacterController character;  // On XR Origin

    [Header("Input Actions")]
    [SerializeField] private InputActionReference move; // Vector2
    [SerializeField] private InputActionReference turn; // Vector2 or float

    [Header("Tuning")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float turnSpeed = 60f;      // degrees/sec
    [SerializeField] private float gravity = -9.81f;

    private float yVelocity;

    private void Reset()
    {
        character = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        move?.action.Enable();
        turn?.action.Enable();
    }

    private void OnDisable()
    {
        move?.action.Disable();
        turn?.action.Disable();
    }

    private void Update()
    {
        if (!head || !character) return;

        // Match CharacterController height/center to head position (basic VR CC setup)
        Vector3 headLocal = transform.InverseTransformPoint(head.position);
        float height = Mathf.Clamp(headLocal.y, 1.0f, 2.2f);
        character.height = height;
        character.center = new Vector3(headLocal.x, height * 0.5f, headLocal.z);

        // Read move axis
        Vector2 moveAxis = move != null ? move.action.ReadValue<Vector2>() : Vector2.zero;

        // Move relative to head yaw
        Vector3 forward = new Vector3(head.forward.x, 0f, head.forward.z).normalized;
        Vector3 right = new Vector3(head.right.x, 0f, head.right.z).normalized;
        Vector3 moveDir = (forward * moveAxis.y + right * moveAxis.x);

        // Gravity
        if (character.isGrounded && yVelocity < 0f) yVelocity = -1f;
        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = yVelocity;

        character.Move(velocity * Time.deltaTime);

        // Turn input (supports Vector2 or float bindings)
        float yawInput = 0f;
        if (turn != null)
        {
            if (turn.action.activeValueType == typeof(Vector2))
                yawInput = turn.action.ReadValue<Vector2>().x;
            else
                yawInput = turn.action.ReadValue<float>();
        }

        if (Mathf.Abs(yawInput) > 0.01f)
            transform.Rotate(0f, yawInput * turnSpeed * Time.deltaTime, 0f);
    }
}