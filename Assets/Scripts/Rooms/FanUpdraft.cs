using UnityEngine;

public class FanUpdraft : MonoBehaviour
{
    public float pushForce = 10f;

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;

        if (rb != null)
        {
            rb.AddForce(Vector2.up * pushForce);
        }
    }

    
        
    
}
