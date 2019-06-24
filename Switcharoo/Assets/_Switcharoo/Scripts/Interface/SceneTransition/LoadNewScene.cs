using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
public class LoadNewScene : MonoBehaviour
{
    public void LoadScene(string p_newScene)
    {
        SceneManager.LoadScene(p_newScene);
    }
    public void LoadScene(int p_sceneIndex)
    {
        SceneManager.LoadScene(p_sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    
}
