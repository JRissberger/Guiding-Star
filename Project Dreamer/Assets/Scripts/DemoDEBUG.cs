using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoDEBUG : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSound()
    {
        SceneManager.LoadScene("AISoundTest");
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
}
