using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    // Fast skrå vinkel (X-akse). Du kan nu ændre denne live i Inspectoren!
    public float tiltAngle = 10f;

    public float moveSmoothSpeed = 5f;
    public float rotateSmoothSpeed = 5f;

    private bool isUpsideDown = false;
    private Quaternion targetRotation;

    // Gemmer kameraets oprindelige horisontale retning (Y-akse)
    private float initialYRotation;

    private void Start()
    {
        // Gemmer den Y-rotation, du har sat kameraet til i Inspectoren
        initialYRotation = transform.eulerAngles.y;

        UpdateTargetRotation();
        transform.rotation = targetRotation;
    }

    private void OnEnable()
    {
        if (PlayerEvents.instance != null)
        {
            PlayerEvents.instance.GravityEvent.AddListener(FlipCamera);
        }
    }

    private void OnDisable()
    {
        if (PlayerEvents.instance != null)
        {
            PlayerEvents.instance.GravityEvent.RemoveListener(FlipCamera);
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // NYT: Vi kalder denne hver frame. 
        // Nu reagerer kameraet øjeblikkeligt, hvis du ændrer 'tiltAngle' i Inspectoren.
        UpdateTargetRotation();

        // Offset roteres baseret på kameraets rotation (Z-flip, X-tilt OG Y-retning)
        Vector3 rotatedOffset = targetRotation * offset;

        Vector3 targetPosition = player.position + rotatedOffset;

        // Smooth bevægelse
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSmoothSpeed * Time.deltaTime);

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSmoothSpeed * Time.deltaTime
        );
    }

    void FlipCamera()
    {
        isUpsideDown = !isUpsideDown;
        // Vi behøver ikke længere kalde UpdateTargetRotation() her, 
        // da den nu bliver kaldt automatisk i LateUpdate.
    }

    void UpdateTargetRotation()
    {
        float zRotation = isUpsideDown ? 180f : 0f;

        // Låser kameraet til din tiltAngle, den gemte Y-rotation og den aktuelle Z-rotation.
        targetRotation = Quaternion.Euler(tiltAngle, initialYRotation, zRotation);
    }
}