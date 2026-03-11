using UnityEngine;

public class HideWallsInFront : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Træk din spiller ind her")]
    public Transform player;

    [Tooltip("Sæt dette til det 'Layer' dine vægge har (f.eks. 'Wall')")]
    public LayerMask wallLayer;

    // Holder styr på den væg, vi lige nu har gjort usynlig
    private MeshRenderer hiddenWall;

    private void Update()
    {
        if (player == null) return;

        // Udregn retningen og afstanden fra kameraet til spilleren
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit hit;

        // Skyder laseren. Den tjekker KUN objekter, der er på vores 'wallLayer'
        if (Physics.Raycast(transform.position, direction, out hit, distance, wallLayer))
        {
            MeshRenderer wallRenderer = hit.collider.GetComponent<MeshRenderer>();

            if (wallRenderer != null)
            {
                // Hvis vi allerede har skjult en ANDEN væg, så vis den igen
                if (hiddenWall != null && hiddenWall != wallRenderer)
                {
                    hiddenWall.enabled = true;
                }

                // Gem den nye væg og gør den usynlig
                hiddenWall = wallRenderer;
                hiddenWall.enabled = false;
            }
        }
        else
        {
            // Hvis laseren IKKE rammer en væg længere, så vis den gamle væg igen
            if (hiddenWall != null)
            {
                hiddenWall.enabled = true;
                hiddenWall = null;
            }
        }
    }
}