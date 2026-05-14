using UnityEngine;
using System.Collections;

public class CrownController : MonoBehaviour
{
    [Header("Target positions (siblings of Crown under UI)")]
    public Transform p1Target;
    public Transform p2Target;

    [Header("Offset from target center (local units)")]
    public Vector2 offset = new Vector2(0f, 55f);

    [Header("Idle bob")]
    public float bobAmplitude = 3f;
    public float bobSpeed = 2f;

    [Header("Animation")]
    public float animDuration = 0.4f;

    SpriteRenderer spriteRenderer;
    Coroutine moveCoroutine;
    int lastWinner = -2;
    Vector3 baseLocalPos;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }

    void Update()
    {
        if (spriteRenderer == null || !spriteRenderer.enabled) return;
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.localPosition = baseLocalPos + new Vector3(0f, bob, 0f);
    }

    public void Refresh(int p1Strokes, int p2Strokes)
    {
        int winner;
        if (p1Strokes == p2Strokes) winner = -1;
        else if (p1Strokes < p2Strokes) winner = 0;
        else winner = 1;

        if (winner == lastWinner) return;
        lastWinner = winner;

        if (winner == -1)
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            spriteRenderer.enabled = false;
            return;
        }

        Vector3 rawLocal = winner == 0 ? p1Target.localPosition : p2Target.localPosition;
        Vector3 target = rawLocal + new Vector3(offset.x, offset.y, 0f);

        if (!spriteRenderer.enabled)
        {
            baseLocalPos = target;
            spriteRenderer.enabled = true;
            return;
        }

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(SlideTo(target));
    }

    IEnumerator SlideTo(Vector3 target)
    {
        Vector3 start = baseLocalPos;
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            baseLocalPos = Vector3.Lerp(start, target, t);
            yield return null;
        }
        baseLocalPos = target;
    }
}
