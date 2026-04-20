using System.Collections.Generic;
using UnityEngine;

public class HideWallsInFront : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LayerMask wallLayer;

    [Tooltip("Optional. If assigned, casts use this collider's bounds for better accuracy.")]
    public Collider playerCollider;

    [Header("Cast Settings")]
    [Tooltip("Makes each ray thicker. Keep this small, or nearby buildings may hide wrongly.")]
    public float castRadius = 0.25f;

    [Tooltip("Maximum distance the cast is allowed to check, even if the player is farther away.")]
    public float maxCastDistance = 6f;

    [Header("Debug")]
    public bool showGizmos = true;
    public Color gizmoLineColor = Color.yellow;
    public Color gizmoSphereColor = Color.cyan;

    private HashSet<MeshRenderer> hiddenWalls = new HashSet<MeshRenderer>();

    private void Update()
    {
        if (player == null) return;

        HashSet<MeshRenderer> wallsHitThisFrame = new HashSet<MeshRenderer>();
        Vector3[] samplePoints = GetPlayerSamplePoints();

        foreach (Vector3 targetPoint in samplePoints)
        {
            Vector3 toTarget = targetPoint - transform.position;
            float realDistance = toTarget.magnitude;

            if (realDistance <= 0.01f)
                continue;

            Vector3 direction = toTarget.normalized;

            // Limit how far the cast is allowed to go
            float castDistance = Mathf.Min(realDistance, maxCastDistance);

            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                castRadius,
                direction,
                castDistance,
                wallLayer,
                QueryTriggerInteraction.Ignore
            );

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                MeshRenderer[] renderers = hit.collider.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer r in renderers)
                {
                    if (r != null)
                    {
                        wallsHitThisFrame.Add(r);
                    }
                }
            }
        }

        foreach (MeshRenderer wall in wallsHitThisFrame)
        {
            if (wall != null)
            {
                wall.enabled = false;
            }
        }

        foreach (MeshRenderer wall in hiddenWalls)
        {
            if (wall != null && !wallsHitThisFrame.Contains(wall))
            {
                wall.enabled = true;
            }
        }

        hiddenWalls = wallsHitThisFrame;
    }

    private Vector3[] GetPlayerSamplePoints()
    {
        if (playerCollider != null)
        {
            Bounds b = playerCollider.bounds;

            Vector3 center = b.center;
            Vector3 up = Vector3.up * (b.extents.y * 0.9f);
            Vector3 camRight = transform.right * (b.extents.x * 0.8f);

            return new Vector3[]
            {
                center,
                center + up,
                center - up,
                center + camRight,
                center - camRight
            };
        }
        else
        {
            Vector3 center = player.position;
            Vector3 up = Vector3.up * 1.0f;
            Vector3 camRight = transform.right * 0.4f;

            return new Vector3[]
            {
                center,
                center + up,
                center - up,
                center + camRight,
                center - camRight
            };
        }
    }

    private void OnDisable()
    {
        foreach (MeshRenderer wall in hiddenWalls)
        {
            if (wall != null)
            {
                wall.enabled = true;
            }
        }

        hiddenWalls.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || player == null) return;

        Vector3[] samplePoints = GetPlayerSamplePointsForGizmos();

        foreach (Vector3 targetPoint in samplePoints)
        {
            Vector3 toTarget = targetPoint - transform.position;
            float realDistance = toTarget.magnitude;

            if (realDistance <= 0.01f)
                continue;

            Vector3 direction = toTarget.normalized;
            float castDistance = Mathf.Min(realDistance, maxCastDistance);
            Vector3 endPoint = transform.position + direction * castDistance;

            Gizmos.color = gizmoLineColor;
            Gizmos.DrawLine(transform.position, endPoint);

            Gizmos.color = gizmoSphereColor;
            Gizmos.DrawWireSphere(transform.position, castRadius);
            Gizmos.DrawWireSphere(endPoint, castRadius);
        }
    }

    private Vector3[] GetPlayerSamplePointsForGizmos()
    {
        if (playerCollider != null)
        {
            Bounds b = playerCollider.bounds;

            Vector3 center = b.center;
            Vector3 up = Vector3.up * (b.extents.y * 0.9f);
            Vector3 camRight = transform.right * (b.extents.x * 0.8f);

            return new Vector3[]
            {
                center,
                center + up,
                center - up,
                center + camRight,
                center - camRight
            };
        }
        else if (player != null)
        {
            Vector3 center = player.position;
            Vector3 up = Vector3.up * 1.0f;
            Vector3 camRight = transform.right * 0.4f;

            return new Vector3[]
            {
                center,
                center + up,
                center - up,
                center + camRight,
                center - camRight
            };
        }

        return new Vector3[0];
    }
}