using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public float startPosX;
    public float startPosY;
    public float length;
    public float height;
    public GameObject cam;
    public float parallaxEffectX;
    public float parallaxEffectY;

    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            length = sr.bounds.size.x;
            height = sr.bounds.size.y;
        }
    }

    void Update()
    {
        // Calculate the target position based on the camera's position and parallax effect
        float targetX = cam.transform.position.x * parallaxEffectX;
        float targetY = cam.transform.position.y * parallaxEffectY;

        // Set the background position relative to its starting position
        transform.position = new Vector3(startPosX + targetX, startPosY + targetY, transform.position.z);

        // Calculate camera's relative position for looping
        float cameraRelativePosX = cam.transform.position.x * (1 - parallaxEffectX);
        float cameraRelativePosY = cam.transform.position.y * (1 - parallaxEffectY);

        // Horizontal looping
        while (cameraRelativePosX > startPosX + length)
        {
            startPosX += length;
        }
        while (cameraRelativePosX < startPosX - length)
        {
            startPosX -= length;
        }

        // Vertical looping
        while (cameraRelativePosY > startPosY + height)
        {
            startPosY += height;
        }
        while (cameraRelativePosY < startPosY - height)
        {
            startPosY -= height;
        }
    }
}