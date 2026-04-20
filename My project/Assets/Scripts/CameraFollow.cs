using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    [Header("Rotation Settings")]
    public float tiltAngle = 20f;

    public float moveSmoothSpeed = 5f;
    public float rotateSmoothSpeed = 5f;

    private bool isUpsideDown = false;
    private Quaternion targetRotation;
    private float initialYRotation;

    private void Start()
    {
        initialYRotation = transform.eulerAngles.y;
        UpdateTargetRotation();
        transform.rotation = targetRotation;
    }

    private void OnEnable()
    {
        if (PlayerEvents.instance != null)
            PlayerEvents.instance.GravityEvent.AddListener(FlipCamera);
    }

    private void OnDisable()
    {
        if (PlayerEvents.instance != null)
            PlayerEvents.instance.GravityEvent.RemoveListener(FlipCamera);
    }

    void LateUpdate()
    {
        if (player == null) return;

        UpdateTargetRotation();

        Vector3 rotatedOffset = targetRotation * offset;
        Vector3 targetPosition = player.position + rotatedOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSmoothSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSmoothSpeed * Time.deltaTime
        );
    }

    void FlipCamera()
    {
        isUpsideDown = !isUpsideDown;
    }

    void UpdateTargetRotation()
    {
        float zRotation = isUpsideDown ? 180f : 0f;
        float currentTilt = isUpsideDown ? -tiltAngle : tiltAngle;

        targetRotation = Quaternion.Euler(currentTilt, initialYRotation, zRotation);
    }
}