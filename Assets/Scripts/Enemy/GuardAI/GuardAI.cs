using UnityEngine;

public class GuardAI : MonoBehaviour
{
    public float moveSpeed = 100f; // Base speed, adjustable in Inspector
    public float chaseRadius = 5f; // Radius to detect and chase enemies or follow player
    public float stopDistance = 0.1f; // Minimum distance for Enemy to ensure contact
    public float playerStopDistance = 2.5f; // Safe distance for Player
    public float jumpForce = 10f; // Increased to clear taller obstacles
    public int maxHealth = 3;
    public int damage = 1;
    public float damageCooldown = 1f;

    public Vector2 obstacleCheckSize = new Vector2(0.5f, 0.8f); // Adjusted height to match sprite, width as is
    public LayerMask groundLayer; // Assign "Ground" layer here in Inspector

    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;
    private float lastDamageTime;
    private bool isLocked = true; // Starts locked until Player touch
    private Transform target; // Can be Player or Enemy
    private float lastJumpTime; // To prevent rapid jumps
    private float jumpCooldown = 0.5f; // Cooldown between jumps

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.linearVelocity = Vector2.zero; // Ensure it starts stationary
        rb.linearDamping = 0f; // Set drag to 0 for maximum speed responsiveness
        rb.angularDamping = 0f; // Prevent rotational resistance
        rb.gravityScale = 1f; // Ensure gravity is normal, adjust if needed
        Debug.Log($"GuardAI {gameObject.name} started, isLocked: {isLocked}, Tag: {gameObject.tag}, moveSpeed: {moveSpeed}");
    }

    private void FixedUpdate()
    {
        if (isDead || isLocked)
        {
            Debug.Log($"GuardAI {gameObject.name} is idle: isDead={isDead}, isLocked={isLocked}");
            return;
        }

        // Re-target every frame to check for enemies within radius
        FindTarget();

        if (target == null)
        {
            Debug.Log($"GuardAI {gameObject.name} has no target");
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);
        float currentStopDistance = (target.CompareTag("Enemy")) ? stopDistance : playerStopDistance;
        Debug.Log($"GuardAI {gameObject.name} distance to target: {distance}, Target Tag: {target.gameObject.tag}, Stop Distance: {currentStopDistance}");

        if (distance <= chaseRadius && distance > currentStopDistance)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y); // Set full velocity

            // Flip sprite
            if (direction.x > 0)
                transform.localScale = new Vector3(5, 5, 1);
            else
                transform.localScale = new Vector3(-5, 5, 1);

            animator.SetBool("moving", true);

            // Check for obstacle ahead
            Vector2 castDirection = new Vector2(Mathf.Sign(direction.x), 0);
            Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;

            RaycastHit2D hit = Physics2D.BoxCast(origin, obstacleCheckSize, 0, castDirection, 0.5f, groundLayer); // Increased distance to 0.5f
            if (hit.collider != null)
            {
                if (Time.time - lastJumpTime > jumpCooldown)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply jump force
                    animator.SetTrigger("jump");
                    lastJumpTime = Time.time;
                    Debug.Log($"GuardAI {gameObject.name} jumping over obstacle at {hit.collider.name}");
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement
            animator.SetBool("moving", false);
        }
    }

    private void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform nearest = null;
        float shortestDistance = Mathf.Infinity;

        // Prioritize chasing Enemy with Health within chaseRadius
        if (enemies.Length > 0)
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy != gameObject) // Avoid targeting self
                {
                    float dist = Vector2.Distance(transform.position, enemy.transform.position);
                    if (dist <= chaseRadius)
                    {
                        Health enemyHealth = enemy.GetComponent<Health>();
                        if (enemyHealth != null) // Only target enemies with Health
                        {
                            if (dist < shortestDistance)
                            {
                                shortestDistance = dist;
                                nearest = enemy.transform;
                            }
                        }
                    }
                }
            }
        }

        // Fallback to following Player if no Enemy with Health within radius
        if (nearest == null && players.Length > 0)
        {
            foreach (GameObject player in players)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist <= chaseRadius && dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearest = player.transform;
                }
            }
        }

        target = nearest;
        Debug.Log($"GuardAI {gameObject.name} target set to: {target?.gameObject.name ?? "null"}, Tag: {target?.gameObject.tag ?? "null"}, Distance: {shortestDistance}");
    }

    private bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.6f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.2f, groundLayer);
        return hit.collider != null;
    }

    public void TakeDamage(int damageAmount)
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage((float)damageAmount);
            animator.SetTrigger("hurt");
        }

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
        Gizmos.DrawWireCube(origin + direction * 0.5f, obstacleCheckSize); // Match BoxCast distance
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("die");
        Destroy(gameObject, 1.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"GuardAI {gameObject.name} collision with: {collision.gameObject.name}, Tag: {collision.gameObject.tag}, isLocked: {isLocked}, Contact Points: {collision.contactCount}");
        if (isLocked && collision.gameObject.CompareTag("Player"))
        {
            isLocked = false;
            Debug.Log($"GuardAI {gameObject.name} unlocked and activated!");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || isLocked) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (Time.time - lastDamageTime > damageCooldown)
            {
                Health enemyHealth = collision.gameObject.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((float)damage); // Convert int to float
                    Debug.Log($"GuardAI {gameObject.name} dealt {damage} damage to {collision.gameObject.name}");
                    lastDamageTime = Time.time;
                }
                else
                {
                    Debug.LogWarning($"GuardAI {gameObject.name} target {collision.gameObject.name} has no Health component");
                }
            }
        }
    }
}