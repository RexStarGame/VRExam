using UnityEngine;
using System.Collections;

public class ColiderCheck : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {

        HitDirection hitDir = ReturnDirection(collision);

        if (hitDir != HitDirection.Top && hitDir != HitDirection.Bottom && hitDir != HitDirection.None)
        {
            //Destroy(collision.gameObject);  // spiller dør 
        }
    }

    private enum HitDirection { None, Top, Bottom, Forward, Back, Left, Right }

    private HitDirection ReturnDirection(Collision collision)
    {

        HitDirection hitDirection = HitDirection.None;

        Vector3 normal = collision.GetContact(0).normal;

        if (Vector3.Angle(normal, transform.up) < 45f) { hitDirection = HitDirection.Top; }
        else if (Vector3.Angle(normal, -transform.up) < 45f) { hitDirection = HitDirection.Bottom; }
        else if (Vector3.Angle(normal, transform.forward) < 45f) { hitDirection = HitDirection.Forward; }
        else if (Vector3.Angle(normal, -transform.forward) < 45f) { hitDirection = HitDirection.Back; }
        else if (Vector3.Angle(normal, transform.right) < 45f) { hitDirection = HitDirection.Right; }
        else if (Vector3.Angle(normal, -transform.right) < 45f) { hitDirection = HitDirection.Left; }

        return hitDirection;
    }
}