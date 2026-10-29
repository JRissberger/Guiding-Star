using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for the Core Loop PrototypeMenu
/// </summary>
public class CoreLoopMenu : MonoBehaviour 
{
    public void OpenCoreLoop()
    {
        SceneManager.LoadScene("CoreLoopPrototypeScene");
    }

    public void OpenArtMuseum()
    {
        SceneManager.LoadScene("ArtMuseumTest");
    }

    public void OpenCoreLoopMenu()
    {
        SceneManager.LoadScene("CoreLoopMenu");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
