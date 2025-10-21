using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    [Header("Dimensions")]
    public float width = 1.0f;
    public float height = 1.0f;
    public Vector3 offset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Properties")]
    [Range(0f, 1f)]
    public float flapFraction;
    public AirfoilConfig airfoilConfig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        float nonFlappedHeight = height * (1 - flapFraction);
        Vector3 offsetNoFlap = new(height/2 - nonFlappedHeight / 2, 0f, 0f);
        Gizmos.color = new Color(110, 186, 212, 120) / 255f;
        Gizmos.DrawCube(transform.position + offset + offsetNoFlap, new Vector3(nonFlappedHeight, 0.1f, width));

        float flappedHeight = height * flapFraction;
        Vector3 offsetFlap = new(height / 2 - flappedHeight / 2, 0f, 0f);
        Gizmos.color = new Color(235, 129, 73, 120) / 255f;
        Gizmos.DrawCube(transform.position + offset - offsetFlap, new Vector3(flappedHeight, 0.1f, width));
    }
}
