using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TurnTextUI : MonoBehaviour
{
    public TMP_Text turnText;

    [Header("Fade")]
    public float fadeInDuration = 0.2f;

    string p1Name;
    string p2Name;
    Coroutine current;

    TMP_Text[]       tmpTexts;
    Image[]          images;
    SpriteRenderer[] sprites;

    void Awake()
    {
        p1Name = PlayerPrefs.GetString("Player1Name", "Jogador 1");
        p2Name = PlayerPrefs.GetString("Player2Name", "Jogador 2");

        tmpTexts = GetComponentsInChildren<TMP_Text>(true);
        images   = GetComponentsInChildren<Image>(true);
        sprites  = GetComponentsInChildren<SpriteRenderer>(true);

        SetAlpha(0f);
    }

    public void ShowP1Turn()              => Show($"Vez de {p1Name}");
    public void ShowP2Turn()              => Show($"Vez de {p2Name}");
    public void ShowP1ObstacleSelection() => Show($"Preparo de {p1Name}");
    public void ShowP2ObstacleSelection() => Show($"Preparo de {p2Name}");

    void Show(string message)
    {
        turnText.text = message;
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        yield return Fade(0f, 1f, fadeInDuration);
        // stays visible until the next Show() call
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float a)
    {
        foreach (var t in tmpTexts) { var c = t.color; c.a = a; t.color = c; }
        foreach (var i in images)   { var c = i.color; c.a = a; i.color = c; }
        foreach (var s in sprites)  { var c = s.color; c.a = a; s.color = c; }
    }
}
