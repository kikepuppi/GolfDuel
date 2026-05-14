using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public Button resumeButton;
    public Button menuButton;
    public CameraFollow cameraFollow;

    bool isPaused = false;

    void Start() {
        pausePanel.SetActive(false);
        resumeButton.onClick.AddListener(Retomar);
        menuButton.onClick.AddListener(VoltarAoMenu);
    }

    void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (isPaused) Retomar();
            else Pausar();
        }
    }

    public void Pausar() {
        isPaused = true;
        pausePanel.SetActive(true);
        cameraFollow.SetPaused(true);
    }

    public void Retomar() {
        isPaused = false;
        pausePanel.SetActive(false);
        cameraFollow.SetPaused(false);
    }

    public void VoltarAoMenu() {
        SceneManager.LoadScene("Menu");
    }
}
