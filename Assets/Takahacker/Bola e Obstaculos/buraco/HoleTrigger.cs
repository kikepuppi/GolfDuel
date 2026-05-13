using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    [Range(0f, 1f)]
    public float requiredCoverage = 0.8f;

    CircleCollider2D myCol;

    void Awake()
    {
        myCol = GetComponent<CircleCollider2D>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Ball")) return;

        CircleCollider2D ballCol = col as CircleCollider2D;
        if (ballCol == null) return;

        float rHole = myCol.bounds.extents.x;
        float rBall = ballCol.bounds.extents.x;
        float d = Vector2.Distance(myCol.bounds.center, col.bounds.center);

        float overlap = CircleOverlapArea(rHole, rBall, d);
        float holeArea = Mathf.PI * rHole * rHole;
        float coverage = overlap / holeArea;
        Debug.Log($"coverage={coverage:F2}");
        if (coverage < requiredCoverage) return;
        Debug.Log("BURACO!");

        var golfInput = col.GetComponent<GolfInput>();
        if (golfInput == null) return;

        golfInput.SetFinished();
        col.gameObject.SetActive(false);
        SoundManager.Instance?.PlayHoleSound();
        GameManager.Instance?.OnBallHoled(golfInput.playerIndex);
    }

    float CircleOverlapArea(float r1, float r2, float d)
    {
        if (d >= r1 + r2) return 0f;
        float rMin = Mathf.Min(r1, r2);
        if (d + rMin <= Mathf.Max(r1, r2)) return Mathf.PI * rMin * rMin;

        float a1 = r1 * r1 * Mathf.Acos((d * d + r1 * r1 - r2 * r2) / (2f * d * r1));
        float a2 = r2 * r2 * Mathf.Acos((d * d + r2 * r2 - r1 * r1) / (2f * d * r2));
        float a3 = 0.5f * Mathf.Sqrt((-d + r1 + r2) * (d + r1 - r2) * (d - r1 + r2) * (d + r1 + r2));
        return a1 + a2 - a3;
    }
}