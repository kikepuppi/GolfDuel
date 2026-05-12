using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObstacleSelectionPanelUI : MonoBehaviour
{
    [Tooltip("Os 5 GameObjects de obstáculo (filhos de entryContainer)")]
    public GameObject[] obstacleEntries;

    [Tooltip("Filho do painel que contém APENAS os 5 entries — sem título")]
    public Transform entryContainer;

    [Header("Espaçamento entre entries")]
    public float spacing = 10f;

    void Awake()
    {
        // VLG vai no container, não no painel — o título fica de fora
        Transform target = entryContainer != null ? entryContainer : transform;

        var vlg = target.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = target.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing               = spacing;
        vlg.childAlignment        = TextAnchor.UpperCenter;
        vlg.childControlWidth     = false;
        vlg.childForceExpandWidth = false;
        vlg.childControlHeight    = false;
        vlg.childForceExpandHeight = false;

        gameObject.SetActive(false);
    }

    // Conecte APENAS este método ao UnityEvent — sem SetActive junto
    public void Show()
    {
        Debug.Log($"[ObstaclePanel] Show() chamado em {gameObject.name}. entries={obstacleEntries?.Length}");

        if (obstacleEntries == null || obstacleEntries.Length == 0)
        {
            Debug.LogWarning($"[ObstaclePanel] obstacleEntries está vazio em {gameObject.name}!");
            gameObject.SetActive(true);
            return;
        }

        // Desativa todos enquanto o painel ainda está inativo
        foreach (var e in obstacleEntries)
            if (e != null) e.SetActive(false);

        // Embaralha
        List<int> idx = new List<int>();
        for (int i = 0; i < obstacleEntries.Length; i++) idx.Add(i);
        for (int i = idx.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = idx[i]; idx[i] = idx[j]; idx[j] = tmp;
        }

        // Ativa exatamente 3
        int activated = 0;
        for (int i = 0; i < 3 && i < idx.Count; i++)
        {
            if (obstacleEntries[idx[i]] != null)
            {
                obstacleEntries[idx[i]].SetActive(true);
                Debug.Log($"[ObstaclePanel] Ativou entry: {obstacleEntries[idx[i]].name}");
                activated++;
            }
        }

        Debug.Log($"[ObstaclePanel] {activated} entries ativados. Ativando painel.");
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}
