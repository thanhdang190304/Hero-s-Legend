using UnityEngine;

public class Summon : MonoBehaviour
{
    public int damage = 1;
    public float duration = 1f; // destroy after 1 sec
    public LayerMask playerLayer;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
