using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteAnimation : MonoBehaviour
{
    public Image imageDisplay;
    public Sprite[] frames;
    public float frameDelay = 0.1f;
    public bool loop = true;

    Coroutine currentAnimation;

    public void Play() {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(Animate());
    }

    public void Stop() {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        imageDisplay.gameObject.SetActive(false);
    }

    IEnumerator Animate() {
        imageDisplay.gameObject.SetActive(true);
        do {
            foreach (Sprite frame in frames) {
                imageDisplay.sprite = frame;
                yield return new WaitForSeconds(frameDelay);
            }
        } while (loop);
    }
}
