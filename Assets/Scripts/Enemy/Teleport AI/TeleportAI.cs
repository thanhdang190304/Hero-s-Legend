using UnityEngine;

public class TeleportAI : MonoBehaviour
{
    [Header("Movement & Combat")]
    public float teleportDistance = 10f;
    public float chaseDistance = 5f;
    public float speed = 3f;
    public float teleportCooldown = 5f;

    [Header("Health & Damage")]
    public int maxHealth = 3;
    public int damage = 1;
    public float damageCooldown = 1f;

    private float teleportTimer = 0f;
    private float lastDamageTime = 0f;
    private int currentHealth;
    private bool isDead = false;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        teleportTimer = teleportCooldown;
    }

    void Update()
    {
        if (isDead) return;

        player = FindClosestPlayer();
        if (player == null) return;

        teleportTimer += Time.deltaTime;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= teleportDistance && distance > chaseDistance)
        {
            if (teleportTimer >= teleportCooldown)
            {
                TeleportNearPlayer();
                teleportTimer = 0f;
            }
            else
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Teleportt"))
                {
                    animator.SetBool("moving", true);
                    ChasePlayer();
                }
            }
        }
        else if (distance <= chaseDistance)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Teleportt"))
            {
                animator.SetBool("moving", true);
                ChasePlayer();
            }
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p.transform;
            }
        }

        return closest;
    }

    void TeleportNearPlayer()
    {
        if (player == null) return;

        Vector3 offset = (transform.position.x > player.position.x) ? Vector3.right : Vector3.left;
        transform.position = player.position + offset;

        animator.SetTrigger("teleport");
        animator.SetBool("moving", false);
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (direction.x > 0)
            transform.localScale = new Vector3(5, 5, 1);
        else
            transform.localScale = new Vector3(-5, 5, 1);
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
