using UnityEngine;

public class GrassController : MonoBehaviour
{
    [Header("Referências")]
    public Material grassMaterial;

    [Header("Animação")]
    public float swaySpeed = 2.0f;

    private static readonly int WaveOffsetID = Shader.PropertyToID("_WaveOffset");

    void Update()
    {
        if (grassMaterial == null) return;
        grassMaterial.SetFloat(WaveOffsetID, Time.time * swaySpeed);
    }
}