using UnityEngine;
using System.Collections.Generic;

public class GrassSpawner : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] grassSprites;

    [Header("Área")]
    public Vector2 areaSize = new Vector2(20f, 15f);
    public float minDistance = 0.5f; // distância mínima entre sprigs (Poisson)

    [Header("Tamanho")]
    public float minScale = 0.3f;
    public float maxScale = 0.6f;

    [Header("Animação")]
    public float swaySpeed = 1.5f;
    public float swayAmount = 10f;

    [Header("Sorting")]
    public string sortingLayerName = "Default";
    public int orderInLayer = 1;

    private class GrassSprig
    {
        public Transform tf;
        public float offset;
        public float speedMult;
        public float dir;
    }

    private List<GrassSprig> sprigs = new List<GrassSprig>();

    void Start()
    {
        var points = PoissonDisc(areaSize, minDistance);
        foreach (var p in points)
            SpawnSprig(p);
    }

    void SpawnSprig(Vector2 localPos)
    {
        if (grassSprites == null || grassSprites.Length == 0) return;

        var go = new GameObject("Sprig");
        go.transform.parent = transform;
        go.transform.localPosition = new Vector3(localPos.x, localPos.y, 0);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = grassSprites[Random.Range(0, grassSprites.Length)];
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = orderInLayer;

        // Pivot na base para rotacionar pelo pé
        go.transform.localPosition += new Vector3(0, sr.bounds.extents.y, 0);

        float scale = Random.Range(minScale, maxScale);
        float flipX = Random.value > 0.5f ? 1f : -1f;
        go.transform.localScale = new Vector3(scale * flipX, scale, 1f);

        sprigs.Add(new GrassSprig
        {
            tf = go.transform,
            offset = Random.Range(0f, Mathf.PI * 2f),
            speedMult = Random.Range(0.6f, 1.5f),
            dir = Random.value > 0.5f ? 1f : -1f
        });
    }

    void Update()
    {
        foreach (var s in sprigs)
        {
            if (s.tf == null) continue;
            float angle = Mathf.Sin(Time.time * swaySpeed * s.speedMult + s.offset)
                          * swayAmount * s.dir;
            s.tf.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // Poisson Disc Sampling — distribuição natural sem sobreposição
    List<Vector2> PoissonDisc(Vector2 size, float minDist, int attempts = 15)
    {
        var points = new List<Vector2>();
        var active = new List<Vector2>();
        var cellSize = minDist / Mathf.Sqrt(2);
        int cols = Mathf.CeilToInt(size.x / cellSize);
        int rows = Mathf.CeilToInt(size.y / cellSize);
        var grid = new Vector2?[cols, rows];

        Vector2 first = new Vector2(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2));
        points.Add(first);
        active.Add(first);
        int gx = Mathf.Clamp((int)((first.x + size.x / 2) / cellSize), 0, cols - 1);
        int gy = Mathf.Clamp((int)((first.y + size.y / 2) / cellSize), 0, rows - 1);
        grid[gx, gy] = first;

        while (active.Count > 0)
        {
            int idx = Random.Range(0, active.Count);
            Vector2 current = active[idx];
            bool found = false;

            for (int a = 0; a < attempts; a++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float dist = Random.Range(minDist, minDist * 2f);
                Vector2 candidate = current + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

                if (candidate.x < -size.x / 2 || candidate.x > size.x / 2 ||
                    candidate.y < -size.y / 2 || candidate.y > size.y / 2)
                    continue;

                int cx = Mathf.Clamp((int)((candidate.x + size.x / 2) / cellSize), 0, cols - 1);
                int cy = Mathf.Clamp((int)((candidate.y + size.y / 2) / cellSize), 0, rows - 1);

                bool valid = true;
                for (int nx = Mathf.Max(0, cx - 2); nx <= Mathf.Min(cols - 1, cx + 2) && valid; nx++)
                    for (int ny = Mathf.Max(0, cy - 2); ny <= Mathf.Min(rows - 1, cy + 2) && valid; ny++)
                        if (grid[nx, ny].HasValue &&
                            Vector2.Distance(grid[nx, ny].Value, candidate) < minDist)
                            valid = false;

                if (valid)
                {
                    points.Add(candidate);
                    active.Add(candidate);
                    grid[cx, cy] = candidate;
                    found = true;
                    break;
                }
            }

            if (!found) active.RemoveAt(idx);
        }

        return points;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}