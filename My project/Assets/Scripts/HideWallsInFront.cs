using UnityEngine;

public class HideWallsInFront : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Træk din spiller ind her")]
    public Transform player;

    [Tooltip("Sæt dette til det 'Layer' dine vægge har (f.eks. 'Wall')")]
    public LayerMask wallLayer;

    private MeshRenderer hiddenWall;

    private void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, distance, wallLayer))
        {
            MeshRenderer wallRenderer = hit.collider.GetComponent<MeshRenderer>();

            if (wallRenderer != null)
            {
                if (hiddenWall != null && hiddenWall != wallRenderer)
                {
                    hiddenWall.enabled = true;
                }

                hiddenWall = wallRenderer;
                hiddenWall.enabled = false;
            }
        }
        else
        {
            if (hiddenWall != null)
            {
                hiddenWall.enabled = true;
                hiddenWall = null;
            }
        }
    }
}