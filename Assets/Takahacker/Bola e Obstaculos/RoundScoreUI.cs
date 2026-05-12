using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class RoundScoreUI : MonoBehaviour
{
    [Header("References")]
    public TMP_Text scoreText;
    public Transform bigScoreAnchor; // sibling P1_BIG or P2_BIG

    [Header("Slide-in (from below)")]
    public float slideInDuration = 0.3f;
    public float hiddenYOffset = 150f;

    [Header("Slide-out (into big score)")]
    public float slideOutDuration = 0.25f;

    Vector3 restLocalPos;
    Coroutine current;
    SpriteRenderer[] sprites;
    MeshRenderer[]   meshes;
    int[] savedSpriteOrders;
    int[] savedMeshOrders;

    void Awake()
    {
        restLocalPos = transform.localPosition;
        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        meshes  = GetComponentsInChildren<MeshRenderer>(true);
        savedSpriteOrders = Array.ConvertAll(sprites, s => s.sortingOrder);
        savedMeshOrders   = Array.ConvertAll(meshes,  m => m.sortingOrder);
        gameObject.SetActive(false);
    }

    // First stroke of the round — slides up from below
    public void SlideIn(int strokes)
    {
        if (current != null) StopCoroutine(current);
        RestoreOrders();
        scoreText.text = $"+{strokes}";
        gameObject.SetActive(true);
        Vector3 from = restLocalPos + Vector3.down * hiddenYOffset;
        current = StartCoroutine(AnimateLocal(from, restLocalPos, slideInDuration));
    }

    // Subsequent strokes — just update the number
    public void UpdateScore(int strokes)
    {
        if (gameObject.activeSelf)
            scoreText.text = $"+{strokes}";
    }

    // Round end — slides behind big score then calls onComplete
    public void SlideOut(Action onComplete)
    {
        if (!gameObject.activeSelf) { onComplete?.Invoke(); return; }
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(SlideOutRoutine(onComplete));
    }

    IEnumerator SlideOutRoutine(Action onComplete)
    {
        SetOrdersBehind();
        Vector3 target = bigScoreAnchor != null ? bigScoreAnchor.localPosition : restLocalPos;
        yield return StartCoroutine(AnimateLocal(transform.localPosition, target, slideOutDuration));
        gameObject.SetActive(false);
        transform.localPosition = restLocalPos;
        RestoreOrders();
        onComplete?.Invoke();
    }

    IEnumerator AnimateLocal(Vector3 from, Vector3 to, float duration)
    {
        transform.localPosition = from;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, elapsed / duration));
            yield return null;
        }
        transform.localPosition = to;
    }

    void SetOrdersBehind()
    {
        for (int i = 0; i < sprites.Length; i++) sprites[i].sortingOrder = savedSpriteOrders[i] - 5;
        for (int i = 0; i < meshes.Length;  i++) meshes[i].sortingOrder  = savedMeshOrders[i]   - 5;
    }

    void RestoreOrders()
    {
        for (int i = 0; i < sprites.Length; i++) sprites[i].sortingOrder = savedSpriteOrders[i];
        for (int i = 0; i < meshes.Length;  i++) meshes[i].sortingOrder  = savedMeshOrders[i];
    }
}
