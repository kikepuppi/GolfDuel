using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteAnimation : MonoBehaviour
{
    public Image imageDisplay;
    public float frameDelay = 0.1f;

    Sprite[] frames;
    Coroutine currentAnimation;

    public void Play(Sprite[] newFrames, float delay = 0.1f) {
        frames = newFrames;
        frameDelay = delay;
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(Animate());
    }

    public void Stop() {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        if (imageDisplay != null) imageDisplay.gameObject.SetActive(false);
    }

    IEnumerator Animate() {
        if (frames == null || frames.Length == 0) yield break;
        imageDisplay.gameObject.SetActive(true);
        while (true) {
            foreach (Sprite frame in frames) {
                imageDisplay.sprite = frame;
                yield return new WaitForSeconds(frameDelay);
            }
        }
    }
}
