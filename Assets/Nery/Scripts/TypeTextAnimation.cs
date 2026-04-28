using UnityEngine;
using System.Collections;
using TMPro;

public class TypeTextAnimation : MonoBehaviour
{
    public float typeDelay = 0.05f;
    public TextMeshProUGUI textObject;
    public string fullText;

    public bool isTyping { get; private set; } = false;

    // Remove o Start() — quem inicia é o TutorialSystem
    
    public void StartTyping() {
        StopAllCoroutines();
        StartCoroutine(TypeText());
    }

    // Pula direto para o fim da animação
    public void SkipTyping() {
        StopAllCoroutines();
        textObject.text = fullText;
        textObject.maxVisibleCharacters = fullText.Length;
        isTyping = false;
    }

    IEnumerator TypeText() {
        isTyping = true;
        textObject.text = fullText;
        textObject.maxVisibleCharacters = 0;
        for (int i = 0; i <= textObject.text.Length; i++) {
            textObject.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }
        isTyping = false;
    }
}
