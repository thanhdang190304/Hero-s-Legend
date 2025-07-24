using UnityEngine;
using UnityEngine;

public class NinjaFrogAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float chaseRadius = 5f;
    public float stopDistance = 1.2f;
    public float jumpForce = 5f;
    public int maxHealth = 3;
    public int damage = 1;
    public float damageCooldown = 1f;

    public Vector2 obstacleCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;

    public GameObject clonePrefab;
    public float cloneCooldown = 15f;

    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform targetPlayer;
    private bool isDead = false;
    private float lastDamageTime;
    private float lastCloneTime;

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

        if (distance <= chaseRadius)
        {
            TryClone();

            if (distance > stopDistance)
            {
                Vector2 direction = (targetPlayer.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

                // Flip sprite
                transform.localScale = new Vector3(direction.x > 0 ? 5 : -5, 5, 1);
                animator.SetBool("moving", true);

                // Obstacle detection
                Vector2 castDir = new Vector2(Mathf.Sign(direction.x), 0);
                Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;

                RaycastHit2D hit = Physics2D.BoxCast(origin, obstacleCheckSize, 0, castDir, 0.2f, groundLayer);
                if (hit.collider != null && IsGrounded())
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    animator.SetTrigger("jump");
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                animator.SetBool("moving", false);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("moving", false);
        }
    }

    private void TryClone()
    {
        if (Time.time - lastCloneTime >= cloneCooldown)
        {
            lastCloneTime = Time.time;
            animator.SetTrigger("Clone");

            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnOffset = new Vector3((i == 0 ? -1.5f : 1.5f), 0, 0);
                GameObject clone = Instantiate(clonePrefab, transform.position + spawnOffset, Quaternion.identity);

                // Slight color difference
                SpriteRenderer sr = clone.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(Random.Range(0.4f, 0.8f), Random.Range(0.9f, 1f), Random.Range(0.4f, 0.8f));
                }
            }
        }
    }

    private void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float shortest = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < shortest)
            {
                shortest = dist;
                closest = p.transform;
            }
        }

        targetPlayer = closest;
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
                Health hp = collision.gameObject.GetComponent<Health>();
                if (hp != null)
                {
                    hp.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin + direction * 0.2f, obstacleCheckSize);
    }
}
