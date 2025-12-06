using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public Vector3 rotationoffset = new Vector3(20f, 0f, 0f);
    public Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow: No target assigned!");
            return;
        }
        transform.position = target.position + offset;
        transform.eulerAngles = rotationoffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        transform.eulerAngles = rotationoffset;
    }
}
