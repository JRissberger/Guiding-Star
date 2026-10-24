using UnityEngine;

[ExecuteAlways]
public class SoundBubbleTest : MonoBehaviour
{
    [SerializeField] private float minScale = .2f;
    [SerializeField] private float maxScale = 10f;
    [SerializeField] private float expandSpeed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale += Vector3.one * this.expandSpeed;
        if (this.transform.localScale.x > this.maxScale)
        {
            this.transform.localScale = new(this.minScale,this.minScale,this.minScale);
        }
    }
}
