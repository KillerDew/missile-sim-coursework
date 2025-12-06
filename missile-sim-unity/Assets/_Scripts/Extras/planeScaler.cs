using UnityEngine;

[ExecuteInEditMode]
public class planeScaler : MonoBehaviour
{
    Renderer renderer_;
    Material planeMaterial;

    public float scale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        renderer_ = GetComponent<Renderer>();
        planeMaterial = renderer_.material;
    }

    // Update is called once per frame
    void Update()
    {
        planeMaterial.mainTextureScale = new (scale, scale);
        transform.localScale = new (scale, 1, scale);
    }
}
