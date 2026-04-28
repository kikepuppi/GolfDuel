using UnityEngine;
using UnityEngine.InputSystem;

public enum STATE {
    DISABLED,
    WAITING,
    TYPING
}

public class TutorialSystem : MonoBehaviour
{
    public TutorialData tutorialData;

    int currentText = 0;
    bool finished = false;

    TypeTextAnimation typeText;
    STATE state;

    void Awake() {
        // FindObjectsByType retorna array — pega o primeiro
        typeText = FindObjectsByType<TypeTextAnimation>()[0];
    }

    void Start() {
        state = STATE.WAITING;
        Next(); // Começa já no primeiro texto
    }

    void Update() {
        if (state == STATE.DISABLED) return;

        switch (state) {
            case STATE.WAITING: Waiting(); break;
            case STATE.TYPING:  Typing();  break;
        }
    }

    void Next() {
        if (finished) return;

        typeText.fullText = tutorialData.talkScript[currentText++].text;

        if (currentText == tutorialData.talkScript.Count) finished = true;

        typeText.StartTyping();
        state = STATE.TYPING;
    }

    void Waiting() {
        // Qualquer tecla (ou espaço/enter se preferir) avança
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame) {
            if (finished) {
                state = STATE.DISABLED; // Tutorial acabou
                return;
            }
            Next();
        }
    }

    void Typing() {
        // Durante a digitação: tecla pula a animação, segunda pressão avança
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame) {
            if (typeText.isTyping) {
                typeText.SkipTyping();   // 1ª press: completa o texto
                state = STATE.WAITING;
            }
        }

        // Transição automática quando a animação termina
        if (!typeText.isTyping) {
            state = STATE.WAITING;
        }
    }
}
