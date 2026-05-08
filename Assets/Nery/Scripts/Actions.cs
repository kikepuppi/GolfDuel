using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{

    void Start() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            float worldHeight = Camera.main.orthographicSize * 2f;
            float worldWidth = worldHeight * Camera.main.aspect;
            transform.localScale = new Vector3(
                worldWidth / sr.sprite.bounds.size.x,
                worldHeight / sr.sprite.bounds.size.y,
                1f
            );
        }
    }
    
    public void IniciaConfig()
    {
       SceneManager.LoadScene(1);
    }
    
    public void IniciaJogo()
    {
       SceneManager.LoadScene(2);
    }
    
    public GameObject creditsPanel;
    public GameObject instructionsPanel;

    public void AbreCredits() {
        creditsPanel.SetActive(true);
    }

    public void FechaCredits() {
        creditsPanel.SetActive(false);
    }

    public void AbreInstructions() {
        instructionsPanel.SetActive(true);
    }

    public void FechaInstructions() {
        instructionsPanel.SetActive(false);
    }
    
}
