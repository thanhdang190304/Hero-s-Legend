using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float chaseRadius = 5f;
    public float stopDistance = 1.2f;
    public float jumpForce = 5f;
    public int maxHealth = 3;
    public int damage = 1;
    public float damageCooldown = 1f;

    public Vector2 obstacleCheckSize = new Vector2(0.5f, 0.5f); // Width, height of boxcast
    public LayerMask groundLayer; // Assign "Ground" layer here in Inspector

    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;
    private float lastDamageTime;

    private Transform targetPlayer;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    
    {
        if (isDead) return;

        FindClosestPlayer();

        if (targetPlayer == null) return;

        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        if (distance <= chaseRadius && distance > stopDistance)
        {
            Vector2 direction = (targetPlayer.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

            // Flip sprite
            if (direction.x > 0)
                transform.localScale = new Vector3(5, 5, 1);
            else
                transform.localScale = new Vector3(-5, 5, 1);

            animator.SetBool("moving", true);

            // Check for obstacle ahead
            Vector2 castDirection = new Vector2(Mathf.Sign(direction.x), 0);
            Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f; // Slightly above the ground

            RaycastHit2D hit = Physics2D.BoxCast(origin, obstacleCheckSize, 0, castDirection, 0.2f, groundLayer);
            if (hit.collider != null)
            {
                if (IsGrounded())
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    animator.SetTrigger("jump");
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("moving", false);
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

    private bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.6f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.2f, groundLayer);
        return hit.collider != null;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        animator.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void OnDrawGizmos()
{
    Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(origin + direction * 0.2f, obstacleCheckSize);
}


   private void Die()
{
    isDead = true;
    rb.linearVelocity = Vector2.zero;
    animator.SetTrigger("die");

   

    Destroy(gameObject, 1.5f);
}


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime > damageCooldown)
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
    
} 