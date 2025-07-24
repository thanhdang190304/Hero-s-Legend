using UnityEngine;

public class SplittingSlime : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseRange = 8f;
    public float stopDistance = 0.5f;
    public float jumpForce = 6f;
    public Vector2 obstacleCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;

    [Header("Combat")]
    public int damage = 1;
    public int health = 3;
    public GameObject smallerSlimePrefab;
    public float splitForce = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform targetPlayer;
    private bool isDead = false;
    private Vector3 originalScale;
    private bool canSplit = true; // Flag to control splitting

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (isDead) return;

        FindClosestPlayer();

        if (targetPlayer == null) return;

        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        if (distance <= chaseRange && distance > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMoving();
        }
    }

    private void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float shortestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject player in players)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = player.transform;
            }
        }

        targetPlayer = nearest;
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (targetPlayer.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        animator.Play("Idle");

        Vector2 castDirection = new Vector2(Mathf.Sign(direction.x), 0);
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        RaycastHit2D hit = Physics2D.BoxCast(origin, obstacleCheckSize, 0, castDirection, 0.2f, groundLayer);

        if (hit.collider != null && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.Play("Idle");
    }

    private bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.6f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.2f, groundLayer);
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero; // Stop moving
        animator.Play("Die"); // Play your die animation
        // WAIT for the animation event to call SplitAndDestroy()
    }

    // This function is called by Animation Event
    public void SplitAndDestroy()
    {
        // Only split if allowed
        if (canSplit && smallerSlimePrefab != null)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 offset = (i == 0) ? Vector3.left * 0.2f : Vector3.right * 0.2f;
                GameObject newSlime = Instantiate(smallerSlimePrefab, transform.position + offset, Quaternion.identity);

                // Scale down
                newSlime.transform.localScale = transform.localScale * 0.5f;

                Rigidbody2D newRb = newSlime.GetComponent<Rigidbody2D>();
                if (newRb != null)
                {
                    float xForce = (i == 0) ? -splitForce : splitForce;
                    newRb.linearVelocity = new Vector2(xForce, jumpForce);
                }

                SplittingSlime slimeScript = newSlime.GetComponent<SplittingSlime>();
                if (slimeScript != null)
                {
                    slimeScript.moveSpeed = moveSpeed;
                    slimeScript.jumpForce = jumpForce;
                    slimeScript.damage = damage;
                    slimeScript.health = Mathf.Max(1, health / 2); // Avoid 0 health
                    slimeScript.smallerSlimePrefab = smallerSlimePrefab;
                    slimeScript.canSplit = false; // Prevent further splitting
                }
            }
        }

        Destroy(gameObject); // Finally destroy THIS slime
    }
}