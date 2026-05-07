using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum STATE {
    DISABLED,
    WAITING,
    TYPING
}

public class TutorialSystem : MonoBehaviour
{
    [Header("Data")]
    public TutorialData tutorialData;

    [Header("UI")]
    public GameObject tutorialPanel;
    public Button skipButton;

    [Header("Media")]
    public GameObject mediaPanel;
    public Image imageDisplay;
    public SpriteAnimation spriteAnimation;
    public Animator mediaAnimator;

    [Header("Scene")]
    public string nextSceneName;

    int currentText = 0;
    bool finished = false;

    TypeTextAnimation typeText;
    STATE state;

    void Awake() {
        typeText = FindObjectsByType<TypeTextAnimation>()[0];
    }

    void Start() {
        skipButton.onClick.AddListener(SkipTutorial);
        state = STATE.WAITING;
        Next();
    }

    void Update() {
        if (state == STATE.DISABLED) return;

        switch (state) {
            case STATE.WAITING: Waiting(); break;
            case STATE.TYPING:  Typing();  break;
        }
    }

    // ─── Navegação ────────────────────────────────────────────

    void Next() {
        if (finished) {
            EndTutorial();
            return;
        }

        Tutorial entry = tutorialData.talkScript[currentText++];

        if (currentText == tutorialData.talkScript.Count) finished = true;

        typeText.fullText = entry.text;
        typeText.StartTyping();

        UpdateMedia(entry);

        state = STATE.TYPING;
    }

    void Waiting() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame) {
            Next();
        }
    }

    void Typing() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame) {
            if (typeText.isTyping) {
                typeText.SkipTyping();
                state = STATE.WAITING;
                return;
            }
        }

        if (!typeText.isTyping) {
            state = STATE.WAITING;
        }
    }

    // ─── Mídia ────────────────────────────────────────────────

    void UpdateMedia(Tutorial entry) {
        // Reseta tudo
        imageDisplay.gameObject.SetActive(false);
        spriteAnimation.Stop();

        if (entry.mediaType != MediaType.None) {
            mediaPanel.SetActive(true);
        } else {
            mediaPanel.SetActive(false);
            return;
        }

        switch (entry.mediaType) {
            case MediaType.Image:
                imageDisplay.gameObject.SetActive(true);
                imageDisplay.sprite = entry.image;
                break;

            case MediaType.SpriteAnimation:
                spriteAnimation.Play();
                break;

            case MediaType.Animation:
                imageDisplay.gameObject.SetActive(true);
                if (mediaAnimator != null && !string.IsNullOrEmpty(entry.animationTrigger))
                    mediaAnimator.SetTrigger(entry.animationTrigger);
                break;
        }
    }

    // ─── Skip / Fim ───────────────────────────────────────────

    void SkipTutorial() {
        spriteAnimation.Stop();
        tutorialPanel.SetActive(false);
        SceneManager.LoadScene(nextSceneName);
    }

    void EndTutorial() {
        state = STATE.DISABLED;
        spriteAnimation.Stop();
        tutorialPanel.SetActive(false);
        SceneManager.LoadScene(nextSceneName);
    }
}
