using UnityEngine;
using UnityEngine.Animations;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;
    private float startPos, length;
    public GameObject cam;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public void Update()
    {
        float distance = cam.transform.position.x * parallaxFactor;
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
        float movement = cam.transform.position.x * (1 - parallaxFactor);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
 
}