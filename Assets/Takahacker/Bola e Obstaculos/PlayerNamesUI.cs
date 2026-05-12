using UnityEngine;
using TMPro;

public class PlayerNamesUI : MonoBehaviour
{
    public TMP_Text p1NameText;
    public TMP_Text p2NameText;

    void Start()
    {
        p1NameText.text = PlayerPrefs.GetString("Player1Name", "Jogador 1");
        p2NameText.text = PlayerPrefs.GetString("Player2Name", "Jogador 2");
    }
}
