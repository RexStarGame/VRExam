using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class RollingSpikes : MonoBehaviour
{
    [Header("Spike Colliders")]
    [SerializeField] private List<Collider> spikeColliders = new List<Collider>();

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    [Header("Movement Between A and B")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private bool enableMovement = true;
    [SerializeField] private float reachDistance = 0.02f;

    [Header("Start Position")]
    [SerializeField] private StartMode startMode = StartMode.UseCurrentPosition;

    [Header("Wheel Rotation")]
    [SerializeField] private Vector3 rotationSpeedPerAxis = new Vector3(120f, 180f, 90f);

    private Rigidbody rb;
    private Vector3 currentTarget;
    private bool movingToB = true;

    private float lastHitTime = -1f;
    [SerializeField] private float hitCooldown = 0.1f;

    private enum StartMode
    {
        UseCurrentPosition,
        StartAtPointA,
        StartAtPointB
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        SetStartPosition();
        SetupSpikeRelays();
    }

    private void FixedUpdate()
    {
        RotateWheel();

        if (enableMovement)
            MoveBetweenPoints();
    }

    private void SetStartPosition()
    {
        if (pointA == null || pointB == null)
        {
            currentTarget = transform.position;
            return;
        }

        switch (startMode)
        {
            case StartMode.StartAtPointA:
                transform.position = pointA.position;
                currentTarget = pointB.position;
                movingToB = true;
                break;

            case StartMode.StartAtPointB:
                transform.position = pointB.position;
                currentTarget = pointA.position;
                movingToB = false;
                break;

            case StartMode.UseCurrentPosition:
            default:
                float distanceToA = (transform.position - pointA.position).sqrMagnitude;
                float distanceToB = (transform.position - pointB.position).sqrMagnitude;

                if (distanceToA <= distanceToB)
                {
                    currentTarget = pointB.position;
                    movingToB = true;
                }
                else
                {
                    currentTarget = pointA.position;
                    movingToB = false;
                }
                break;
        }
    }

    private void RotateWheel()
    {
        if (rotationSpeedPerAxis == Vector3.zero)
            return;

        Quaternion deltaRotation = Quaternion.Euler(
            rotationSpeedPerAxis * Time.fixedDeltaTime
        );

        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private void MoveBetweenPoints()
    {
        if (pointA == null || pointB == null)
            return;

        Vector3 nextPosition = Vector3.MoveTowards(
            rb.position,
            currentTarget,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        float sqrDistanceToTarget = (nextPosition - currentTarget).sqrMagnitude;

        if (sqrDistanceToTarget <= reachDistance * reachDistance)
        {
            movingToB = !movingToB;
            currentTarget = movingToB ? pointB.position : pointA.position;
        }
    }

    private void SetupSpikeRelays()
    {
        for (int i = 0; i < spikeColliders.Count; i++)
        {
            Collider spike = spikeColliders[i];

            if (spike == null)
                continue;

            spike.isTrigger = true;

            SpikeTouchRelay relay = spike.GetComponent<SpikeTouchRelay>();

            if (relay == null)
                relay = spike.gameObject.AddComponent<SpikeTouchRelay>();

            relay.SetOwner(this, spike);
        }
    }

    public void HandleSpikeTouch(Collider other, Collider touchedSpike)
    {
        if (Time.time < lastHitTime + hitCooldown)
            return;

        if (!other.CompareTag(playerTag))
            return;

        lastHitTime = Time.time;

        CubeMovement cubeMovement = other.GetComponentInParent<CubeMovement>();
        if (cubeMovement != null)
            cubeMovement.enabled = false;

        Rigidbody playerRb = other.GetComponentInParent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        PlayerEvents.instance.DeathEvent.Invoke();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (moveSpeed < 0f)
            moveSpeed = 0f;

        if (reachDistance < 0.001f)
            reachDistance = 0.001f;

        if (hitCooldown < 0f)
            hitCooldown = 0f;
    }

    [ContextMenu("Auto Fill Spike Colliders From Children")]
    private void AutoFillSpikeCollidersFromChildren()
    {
        spikeColliders.Clear();

        Collider[] allColliders = GetComponentsInChildren<Collider>(true);

        for (int i = 0; i < allColliders.Length; i++)
        {
            if (allColliders[i].transform != transform)
                spikeColliders.Add(allColliders[i]);
        }
    }
#endif
}

[DisallowMultipleComponent]
public sealed class SpikeTouchRelay : MonoBehaviour
{
    private RollingSpikes owner;
    private Collider spikeCollider;

    public void SetOwner(RollingSpikes newOwner, Collider sourceCollider)
    {
        owner = newOwner;
        spikeCollider = sourceCollider;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner == null)
            return;

        owner.HandleSpikeTouch(other, spikeCollider);
    }
}