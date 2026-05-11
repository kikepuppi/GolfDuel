using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public SpriteRenderer background;

    public GameObject menuPanel;
    public GameObject creditsPanel;
    public GameObject instructionsPanel;

    void Start() {
        if (background != null) {
            float worldHeight = Camera.main.orthographicSize * 2f;
            float worldWidth = worldHeight * Camera.main.aspect;
            background.transform.localScale = new Vector3(
                worldWidth / background.sprite.bounds.size.x,
                worldHeight / background.sprite.bounds.size.y,
                1f
            );
        }
    }

    public void IniciaConfig() {
        SceneManager.LoadScene(1);
    }

    public void IniciaJogo() {
        SceneManager.LoadScene(2);
    }

    public void AbreCredits() {
        menuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void FechaCredits() {
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void AbreInstructions() {
        menuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void FechaInstructions() {
        instructionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
