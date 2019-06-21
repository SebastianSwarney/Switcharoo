using UnityEngine.SceneManagement;
using UnityEngine;
public class LoadNewScene : MonoBehaviour
{
    public void LoadScene(string p_newScene)
    {
        SceneManager.LoadScene(p_newScene);
    }
}
