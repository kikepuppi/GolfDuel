using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField inputNomeP1;
    public TMP_InputField inputNickP1;
    public TMP_InputField inputNomeP2;
    public TMP_InputField inputNickP2;

    [Header("Botões")]
    public Button botaoProntoP1;
    public Button botaoProntoP2;
    public GameObject botaoProximo;

    private bool p1Pronto = false;
    private bool p2Pronto = false;

    void Start()
    {
        botaoProximo.SetActive(false);

        botaoProntoP1.interactable = false;
        botaoProntoP2.interactable = false;

        inputNomeP1.onValueChanged.AddListener(delegate { ValidarP1(); });
        inputNickP1.onValueChanged.AddListener(delegate { ValidarP1(); });

        inputNomeP2.onValueChanged.AddListener(delegate { ValidarP2(); });
        inputNickP2.onValueChanged.AddListener(delegate { ValidarP2(); });
    }

    void ValidarP1()
    {
        bool valido = !string.IsNullOrWhiteSpace(inputNomeP1.text) &&
                      !string.IsNullOrWhiteSpace(inputNickP1.text);

        botaoProntoP1.interactable = valido;
    }

    void ValidarP2()
    {
        bool valido = !string.IsNullOrWhiteSpace(inputNomeP2.text) &&
                      !string.IsNullOrWhiteSpace(inputNickP2.text);

        botaoProntoP2.interactable = valido;
    }

    public void ProntoP1()
    {
        if (!botaoProntoP1.interactable) return;

        p1Pronto = true;

        // trava inputs
        inputNomeP1.interactable = false;
        inputNickP1.interactable = false;

        // muda visual do botão
        AlterarVisual(botaoProntoP1, Color.gray);

        ChecarProximo();
    }

    public void ProntoP2()
    {
        if (!botaoProntoP2.interactable) return;

        p2Pronto = true;

        inputNomeP2.interactable = false;
        inputNickP2.interactable = false;

        AlterarVisual(botaoProntoP2, Color.gray);

        ChecarProximo();
    }

    void ChecarProximo()
    {
        if (p1Pronto && p2Pronto)
        {
            botaoProximo.SetActive(true);
        }
    }

    void AlterarVisual(Button botao, Color cor)
    {
        var colors = botao.colors;
        colors.normalColor = cor;
        colors.disabledColor = cor;
        botao.colors = colors;

        botao.interactable = false;
    }
}
