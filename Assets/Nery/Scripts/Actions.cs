using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public void IniciaConfig()
    {
       SceneManager.LoadScene(1);
    }
    
    public void IniciaJogo()
    {
       SceneManager.LoadScene(2);
    }
}
