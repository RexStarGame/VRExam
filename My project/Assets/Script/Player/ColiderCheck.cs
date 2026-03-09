using UnityEngine;
using System.Collections;

public class ColiderCheck : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {

        HitDirection hitDir = ReturnDirection(collision.gameObject, this.gameObject);
        if (hitDir == HitDirection.Left ||
            hitDir == HitDirection.Right ||
            hitDir == HitDirection.Forward ||
            hitDir == HitDirection.Back)
        {
            Destroy(gameObject);
        }
    }

    private enum HitDirection { None, Top, Bottom, Forward, Back, Left, Right }

    private HitDirection ReturnDirection(GameObject Object, GameObject ObjectHit)
    {

        HitDirection hitDirection = HitDirection.None;
        RaycastHit MyRayHit;
        Vector3 direction = (Object.transform.position - ObjectHit.transform.position).normalized;
        Ray MyRay = new Ray(ObjectHit.transform.position, direction);

        if (Physics.Raycast(MyRay, out MyRayHit))
        {

            if (MyRayHit.collider != null)
            {
                Vector3 MyNormal = MyRayHit.normal;

                if (MyNormal == MyRayHit.transform.up) { hitDirection = HitDirection.Top; }
                if (MyNormal == -MyRayHit.transform.up) { hitDirection = HitDirection.Bottom; }
                if (MyNormal == MyRayHit.transform.forward) { hitDirection = HitDirection.Forward; }
                if (MyNormal == -MyRayHit.transform.forward) { hitDirection = HitDirection.Back; }
                if (MyNormal == MyRayHit.transform.right) { hitDirection = HitDirection.Right; }
                if (MyNormal == -MyRayHit.transform.right) { hitDirection = HitDirection.Left; }
            }
        }
        return hitDirection;
    }
}