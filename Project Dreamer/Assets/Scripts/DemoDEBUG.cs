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
        SceneManager.LoadScene("Camera 2 Test");
    }

    public void LoadThrow()
    {
        SceneManager.LoadScene("ThrowingTest");
    }

    public void LoadMenu()
    {
        Debug.Log("Loading menu");
        SceneManager.LoadScene("DemoMenu");
    }
}
