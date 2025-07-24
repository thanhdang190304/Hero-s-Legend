using UnityEngine;

public class NinjaCloneAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float chaseRadius = 5f;
    public float stopDistance = 1.2f;
    public float jumpForce = 5f;
    public float attackCooldown = 1f;
    public float lifetime = 10f;

    public Vector2 obstacleCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;

    public int attackDamage = 1;

    private Rigidbody2D rb;
    private Animator animator;
    private Health health; // Reference to Health component
    private bool isDead = false;
    private float lastAttackTime;
    private float spawnTime;

    private Transform targetEnemy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>(); // Get the Health component
        spawnTime = Time.time;

        // Load attack damage (optional, already set via SkillManager)
        string profileName = PlayerPrefs.GetString("ActiveProfile");
        PlayerProfile profile = SaveSystem.LoadProfile(profileName);
        if (profile != null)
        {
            attackDamage = 1; // Override if needed
        }
    }

    private void Update()
    {
        if (isDead) return;

        // Despawn after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            Die();
            return;
        }

        FindClosestEnemy();

        if (targetEnemy == null)
        {
            animator.SetBool("moving", false);
            Debug.Log("[NinjaCloneAI] No target enemy found");
            return;
        }

        float distance = Vector2.Distance(transform.position, targetEnemy.position);

        if (distance <= chaseRadius && distance > stopDistance)
        {
            Vector2 direction = (targetEnemy.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

            // Flip sprite
            if (direction.x > 0)
                transform.localScale = new Vector3(0.405f, 0.405f, 0.405f);
            else
                transform.localScale = new Vector3(-0.405f, 0.405f, 0.405f);

            animator.SetBool("moving", true);
            Debug.Log("[NinjaCloneAI] Chasing enemy at distance: " + distance);

            // Check for obstacle ahead
            Vector2 castDirection = new Vector2(Mathf.Sign(direction.x), 0);
            Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;

            RaycastHit2D hit = Physics2D.BoxCast(origin, obstacleCheckSize, 0, castDirection, 0.2f, groundLayer);
            if (hit.collider != null)
            {
                if (IsGrounded())
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    animator.SetTrigger("jump");
                    Debug.Log("[NinjaCloneAI] Jumping over obstacle");
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("moving", false);
            Debug.Log("[NinjaCloneAI] Stopped chasing, distance: " + distance);
        }
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("[NinjaCloneAI] Found " + enemies.Length + " objects with Enemy tag");
        float shortestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Health>() != null)
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearest = enemy.transform;
                    Debug.Log("[NinjaCloneAI] Targeting enemy: " + enemy.name + " at distance: " + dist);
                }
            }
        }

        targetEnemy = nearest;
        if (targetEnemy == null)
        {
            Debug.Log("[NinjaCloneAI] No enemies with Health component found");
        }
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

        health.TakeDamage(damageAmount); // Use Health component to handle damage
        Debug.Log("[NinjaCloneAI] Took damage, health: " + health.currentHealth);

        if (health.currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 1.5f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null && Time.time - lastAttackTime > attackCooldown)
            {
                enemyHealth.TakeDamage(attackDamage);
                animator.SetTrigger("attack");
                lastAttackTime = Time.time;
                Debug.Log("[NinjaCloneAI] Attacking enemy: " + collision.gameObject.name + ", damage: " + attackDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin + direction * 0.2f, obstacleCheckSize);
    }
}