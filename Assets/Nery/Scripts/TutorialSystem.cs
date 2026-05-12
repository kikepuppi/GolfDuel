using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

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
    public Button nextButton;

    [Header("Media")]
    public GameObject mediaPanel;
    public Image imageDisplay;
    public Image animationDisplay;
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
        nextButton.onClick.AddListener(OnNextButtonClick);
        state = STATE.WAITING;
    }
    
    public void IniciarTutorial() {
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

    void OnNextButtonClick() {
        if (state == STATE.DISABLED) return;

        if (typeText.isTyping) {
            typeText.SkipTyping();
            state = STATE.WAITING;
        } else {
            Next();
        }
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
        imageDisplay.gameObject.SetActive(false);
        animationDisplay.gameObject.SetActive(false);
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
                float delay = entry.frameDelay > 0 ? entry.frameDelay : 0.1f;
                spriteAnimation.imageDisplay = animationDisplay;
                spriteAnimation.Play(entry.animationFrames, delay);
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
        StartCoroutine(CarregarCena());
    }

    void EndTutorial() {
        state = STATE.DISABLED;
        spriteAnimation.Stop();
        StartCoroutine(CarregarCena());
    }

    IEnumerator CarregarCena() {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f) {
            yield return null;
        }
        op.allowSceneActivation = true;
    }
}
