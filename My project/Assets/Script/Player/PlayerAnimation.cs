using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] public int CubesPerAxis = 8;
    [SerializeField] public float Force = 300f, Radius = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerEvents.instance.DeathEvent.AddListener(OnDeath);
    }
    void OnDeath()
    {
        Main();
    }
    void Main()
    {
        for (int x = 0; x < CubesPerAxis; x++)
        {
            for (int y = 0; y < CubesPerAxis; y++)
            {
                for (int z = 0; z < CubesPerAxis; z++)
                {
                    Vector3 coordinates = new Vector3(x, y, z);
                    CreateCube(coordinates);
                }
            }
        }
        gameObject.SetActive(false);
    }
    void CreateCube(Vector3 coordinates)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Renderer rd = cube.GetComponent<Renderer>();
        rd.material = cube.GetComponent<Material>();

        cube.transform.localScale = transform.localScale / CubesPerAxis;

        Vector3 firstCubePos = transform.position - transform.localScale / 2 + cube.transform.localScale / 2;
        cube.transform.position = firstCubePos + Vector3.Scale(coordinates,cube.transform.localScale);

        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.AddExplosionForce(Force, transform.position, Radius);
    }
}
