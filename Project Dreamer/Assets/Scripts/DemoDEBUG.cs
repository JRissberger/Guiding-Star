using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoDEBUG : MonoBehaviour
{
    public void LoadSound()
    {
        SceneManager.LoadScene("AIItemTest");
    }

    public void LoadCamera()
    {
        SceneManager.LoadScene("Camera Dolly Test");
    }

    public void LoadCamera2()
    {
        SceneManager.LoadScene("Camera ForwardLook");
    }
    public void LoadCamera3()
    {
        SceneManager.LoadScene("Camera FollowLook");
    }

    public void LoadThrow()
    {
        SceneManager.LoadScene("ThrowingTest");
    }

    public void LoadDemo()
    {
        SceneManager.LoadScene("DemoScene");
    }

    public void LoadMenu()
    {
        Debug.Log("Loading menu");
        SceneManager.LoadScene("DemoMenu");
    }

    public void LoadArtMuseum()
    {

        SceneManager.LoadScene("ArtMuseumTest");
    }
}
