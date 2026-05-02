using UnityEngine;

/// <summary>
/// Controla o efeito de sombra de nuvens sobre o tilemap.
/// Coloque este script no mesmo GameObject que tem o SpriteRenderer/MeshRenderer
/// com o material CloudShadow.
/// </summary>
[ExecuteAlways]
public class CloudShadowController : MonoBehaviour
{
    [Header("Referência")]
    [Tooltip("Deixe vazio para pegar automaticamente o material do Renderer neste GameObject")]
    public Material cloudMaterial;

    [Header("Movimento")]
    public Vector2 cloudSpeed  = new Vector2(0.015f,  0.008f);
    public Vector2 cloudSpeed2 = new Vector2(-0.010f, 0.005f);

    [Header("Aparência")]
    [Range(0f, 1f)] public float shadowIntensity = 0.45f;
    [Range(0f, 1f)] public float threshold       = 0.45f;
    [Range(0.01f, 1f)] public float softness     = 0.40f;
    public Color shadowColor = new Color(0.1f, 0.15f, 0.2f, 1f);

    [Header("Tiling")]
    public float tiling  = 1.5f;
    public float tiling2 = 2.2f;

    // IDs cacheados para performance
    static readonly int ID_Speed       = Shader.PropertyToID("_Speed");
    static readonly int ID_Speed2      = Shader.PropertyToID("_Speed2");
    static readonly int ID_ShadowAlpha = Shader.PropertyToID("_ShadowAlpha");
    static readonly int ID_Threshold   = Shader.PropertyToID("_Threshold");
    static readonly int ID_Softness    = Shader.PropertyToID("_Softness");
    static readonly int ID_ShadowColor = Shader.PropertyToID("_ShadowColor");
    static readonly int ID_Tiling      = Shader.PropertyToID("_Tiling");
    static readonly int ID_Tiling2     = Shader.PropertyToID("_Tiling2");

    void Awake()
    {
        if (cloudMaterial == null)
        {
            var r = GetComponent<Renderer>();
            if (r != null) cloudMaterial = r.material;
        }
    }

    void Update()
    {
        if (cloudMaterial == null) return;
        PushProperties();
    }

    void PushProperties()
    {
        cloudMaterial.SetVector(ID_Speed,       new Vector4(cloudSpeed.x,  cloudSpeed.y,  0, 0));
        cloudMaterial.SetVector(ID_Speed2,      new Vector4(cloudSpeed2.x, cloudSpeed2.y, 0, 0));
        cloudMaterial.SetFloat (ID_ShadowAlpha, shadowIntensity);
        cloudMaterial.SetFloat (ID_Threshold,   threshold);
        cloudMaterial.SetFloat (ID_Softness,    softness);
        cloudMaterial.SetColor (ID_ShadowColor, shadowColor);
        cloudMaterial.SetFloat (ID_Tiling,      tiling);
        cloudMaterial.SetFloat (ID_Tiling2,     tiling2);
    }

#if UNITY_EDITOR
    // Atualiza no editor sem entrar em Play Mode
    void OnValidate() { if (cloudMaterial != null) PushProperties(); }
#endif
}