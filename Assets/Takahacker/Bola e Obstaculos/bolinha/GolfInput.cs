using UnityEditor;
using UnityEngine;

public class GolfInput : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D col;

    [Header("Player")]
    public int playerIndex; // 0 = P1, 1 = P2
    public Camera playerCamera;

    [Header("Seta")]
    public Transform arrow;
    public GameObject[] bases;
    public Transform ponta;
    public SpriteRenderer pontaRenderer;

    [Header("For�a")]
    public float maxForce = 10f;
    public float maxDragDistance = 2.5f;

    public bool IsFinished { get; private set; }

    Vector2 startMousePos;
    bool isDragging = false;
    bool shotInProgress = false;

    float[] pontaY = new float[] { 1.4f, 2.1f, 2.8f, 3.5f, 4.2f };

    public Vector3 WorldStartPosition { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        arrow.gameObject.SetActive(false);
        WorldStartPosition = transform.position;
    }

    public void SetFinished()
    {
        IsFinished = true;
        isDragging = false;
        shotInProgress = false;
        if (arrow != null) arrow.gameObject.SetActive(false);
    }

    void Update()
    {
        if (IsFinished) return;

        // Disable collider during placement so obstacles don't interact with the ball
        if (col != null)
        {
            bool placing = GameManager.Instance != null &&
                (GameManager.Instance.CurrentPhase == GamePhase.P1ObstacleSelection ||
                 GameManager.Instance.CurrentPhase == GamePhase.P2ObstacleSelection);
            col.enabled = !placing;
        }

        if (rb.linearVelocity.magnitude > 0.05f)
            return;

        if (shotInProgress)
        {
            shotInProgress = false;
            GameManager.Instance?.OnBallStopped(playerIndex);
            return;
        }

        if (!IsMyTurn()) return;

        if (Input.GetMouseButtonDown(0) && MouseIsOverBall())
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

            Vector2 direction = ((Vector2)ponta.position - (Vector2)transform.position).normalized;
            Vector2 force = direction * (distance / maxDragDistance) * maxForce;

            Vector2 ballPos = transform.position;
            Vector2 pontaPos = ponta.position;
            Vector2 vec = pontaPos - ballPos;
            float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            Vector2 dragDir = dragVector.normalized;
            float dragAngle = Mathf.Atan2(dragDir.y, dragDir.x) * Mathf.Rad2Deg;

            rb.AddForce(force, ForceMode2D.Impulse);

            SoundManager.Instance?.PlayShotSound();
            GameManager.Instance?.AddStroke(playerIndex);

            isDragging = false;
            shotInProgress = true;
            arrow.gameObject.SetActive(false);
        }
    }

    bool IsMyTurn()
    {
        if (GameManager.Instance == null) return true;
        var phase = GameManager.Instance.CurrentPhase;
        return (playerIndex == 0 && phase == GamePhase.P1Turn)
            || (playerIndex == 1 && phase == GamePhase.P2Turn);
    }

    Vector2 GetMouseWorldPos()
    {
        Camera cam = playerCamera != null ? playerCamera : Camera.main;
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    bool MouseIsOverBall()
    {
        return col != null && col.OverlapPoint(GetMouseWorldPos());
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
                    // se for borda ? mant�m preta
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

    public void ResetForNewRound(Vector3 startPosition)
    {
        IsFinished = false;
        isDragging = false;
        shotInProgress = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        WorldStartPosition = startPosition;
        transform.position = startPosition;

        if (col != null) col.enabled = true;
        if (arrow != null) arrow.gameObject.SetActive(false);

        gameObject.SetActive(true);
    }
}