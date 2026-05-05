using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class NoiseTextureGenerator : MonoBehaviour
{
    [MenuItem("Tools/Gerar Noise Textures")]
    static void Generate()
    {
        GenerateNoise("WindNoise",   128, 4f,  0.6f);
        GenerateNoise("Albedo2Noise",128, 3f,  0.5f);
        GenerateNoise("Albedo3Noise",128, 2.5f,0.5f);
        Debug.Log("Noise textures geradas em Assets/Takahacker/Assets/");
    }

    static void GenerateNoise(string name, int size, float scale, float persistence)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.R8, false);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float nx = x / (float)size * scale;
            float ny = y / (float)size * scale;
            float val = Mathf.PerlinNoise(nx, ny) * persistence
                      + Mathf.PerlinNoise(nx * 2, ny * 2) * (1 - persistence);
            pixels[y * size + x] = new Color(val, val, val, 1f);
        }

        tex.SetPixels(pixels);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;

        byte[] bytes = tex.EncodeToPNG();
        string path = $"Assets/Takahacker/Assets/{name}.png";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        // Configura como não-sprite (textura de shader)
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Default;
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Bilinear;
        importer.SaveAndReimport();
    }
}
#endif