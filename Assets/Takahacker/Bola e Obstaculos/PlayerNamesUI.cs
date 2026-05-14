using UnityEngine;
using TMPro;

public class PlayerNamesUI : MonoBehaviour
{
    [Header("HUD names")]
    public TMP_Text p1NameText;
    public TMP_Text p2NameText;

    [Header("Ball names")]
    public TMP_Text p1BallNameText;
    public TMP_Text p2BallNameText;

    void Start()
    {
        string p1 = PlayerPrefs.GetString("Player1Name", "Jogador 1");
        string p2 = PlayerPrefs.GetString("Player2Name", "Jogador 2");

        if (p1NameText != null)     p1NameText.text     = p1;
        if (p2NameText != null)     p2NameText.text     = p2;
        if (p1BallNameText != null) p1BallNameText.text = p1;
        if (p2BallNameText != null) p2BallNameText.text = p2;
    }
}
