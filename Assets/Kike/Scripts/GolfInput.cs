using UnityEngine;

public class GolfInput : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Seta")]
    public Transform arrow;
    public GameObject[] bases;
    public Transform ponta;
    public SpriteRenderer pontaRenderer;

    [Header("Força")]
    public float maxForce = 10f;
    public float maxDragDistance = 2.5f;

    Vector2 startMousePos;
    bool isDragging = false;

    float[] pontaY = new float[] { 1.4f, 2.1f, 2.8f, 3.5f, 4.2f };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        arrow.gameObject.SetActive(false);
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 0.05f)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            startMousePos = GetMouseWorldPos();
            arrow.gameObject.SetActive(true);
        }

        if (isDragging)
        {
            Vector2 currentMousePos = GetMouseWorldPos();
            Vector2 dragVector = startMousePos - currentMousePos;

            float distance = Mathf.Clamp(dragVector.magnitude, 0, maxDragDistance);
            Vector2 direction = dragVector.normalized;

            UpdateArrow(direction, distance);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 endMousePos = GetMouseWorldPos();
            Vector2 dragVector = startMousePos - endMousePos;

            float distance = Mathf.Clamp(dragVector.magnitude, 0, maxDragDistance);

            Vector2 force = dragVector.normalized * (distance / maxDragDistance) * maxForce;

            rb.AddForce(force, ForceMode2D.Impulse);

            isDragging = false;
            arrow.gameObject.SetActive(false);
        }
    }

    Vector2 GetMouseWorldPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void UpdateArrow(Vector2 direction, float distance)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle - 90f);

        float normalized = distance / maxDragDistance;

        UpdateVisuals(normalized);
    }

    void UpdateVisuals(float normalized)
    {
        int level = Mathf.Clamp(Mathf.CeilToInt(normalized * 5f), 1, 5);

        Color color = GetColor(level);

        // --- PONTA ---
        pontaRenderer.color = color;

        Vector3 pos = ponta.localPosition;
        pos.y = pontaY[level - 1];
        ponta.localPosition = pos;

        // --- BASES ---
        int basesAtivas = level - 1;

        for (int i = 0; i < bases.Length; i++)
        {
            if (i < basesAtivas)
            {
                bases[i].SetActive(true);

                // pega TODOS os renderers da base (interior + borda)
                SpriteRenderer[] renderers = bases[i].GetComponentsInChildren<SpriteRenderer>(true);

                foreach (var r in renderers)
                {
                    // se for borda ? mantém preta
                    if (r.gameObject.name.ToLower().Contains("borda"))
                    {
                        r.color = Color.black;
                    }
                    else
                    {
                        r.color = color;
                    }
                }
            }
            else
            {
                bases[i].SetActive(false);
            }
        }
    }

    Color GetColor(int level)
    {
        switch (level)
        {
            case 1: return Hex("#88EB3D");
            case 2: return Hex("#3DD41B");
            case 3: return Hex("#F6DD1E");
            case 4: return Hex("#F6821E");
            case 5: return Hex("#F61E1E");
            default: return Color.white;
        }
    }

    Color Hex(string hex)
    {
        Color c;
        ColorUtility.TryParseHtmlString(hex, out c);
        return c;
    }
}