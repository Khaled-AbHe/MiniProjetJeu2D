using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;
    
    // 0 = moves exactly with the camera (stuck to screen like sky)
    // 1 = doesn't move at all (behaves like foreground objects)
    public float parallaxEffectMultiplier; 

    void Start()
    {
        startPos = transform.position.x;
        // Grabs the horizontal length of the sprite to determine when to loop
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Calculate how much distance we have covered relative to the camera
        float temp = (cam.transform.position.x * (1 - parallaxEffectMultiplier));
        
        // Calculate how much the background should actually move
        float distance = (cam.transform.position.x * parallaxEffectMultiplier);

        // Move the background layer
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        // Check if the camera has moved past the sprite length, if so, warp the position for a seamless loop
        if (temp > startPos + length) 
        {
            startPos += length;
        }
        else if (temp < startPos - length) 
        {
            startPos -= length;
        }
    }
}
