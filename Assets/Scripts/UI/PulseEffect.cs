using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public float pulseSpeed = 1.0f; // Speed of the pulsing effect
    public float scaleFactor = 1.2f; // How much the text scales (e.g., 1.2x original size)

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale; // Store the original scale
    }

    void Update()
    {
        // Calculate the scale based on a sine wave for pulsing effect
        float scale = originalScale.x + Mathf.Sin(Time.time * pulseSpeed) * (scaleFactor - 1);
        transform.localScale = originalScale * scale;
    }
}