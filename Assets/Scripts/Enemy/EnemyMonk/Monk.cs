using UnityEngine;

public class Monk : MonoBehaviour
{
    [Header("Movement & Detection")]
    public float moveSpeed = 2f;
    public float chaseRadius = 6f;
    public float meleeRange = 1.2f;
    public float rangedRange = 4f;
    public float stopBuffer = 0.5f;
    public float jumpForce = 5f;

    [Header("Combat")]
    public int meleeDamage = 1;
    public float summonCooldown = 5f; // Make sure it's set in Inspector
    public float damageCooldown = 1f;
    public GameObject lightningPrefab;

    [Header("Checks")]
    public Vector2 obstacleCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private float cooldownTimer;
    private float lastDamageTime;
    private bool isDead;
    private bool isUsingSkill;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (lightningPrefab == null)
            Debug.LogError("Assign the Lightning prefab to the Monk!");
    }

    private void Update()
    {
        if (isDead) return;

        cooldownTimer += Time.deltaTime;
        player = FindClosestPlayer();

        if (player == null)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        // Flip sprite
        transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);

        // Decision logic
        if (distance <= meleeRange && !isUsingSkill)
        {
            StopMoving();
            animator.SetTrigger("attack");
        }
        else if (distance <= rangedRange && cooldownTimer >= summonCooldown && !isUsingSkill)
        {
            Debug.Log("TRYING TO CAST SKILL");

            StopMoving();
            isUsingSkill = true;
            cooldownTimer = 0f;
            animator.SetTrigger("skill");
        }
        else if (distance <= chaseRadius)
        {
            if (distance > rangedRange - stopBuffer)
            {
                rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
                animator.SetBool("moving", true);

                // Jump over obstacles
                Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
                Vector2 castDir = new Vector2(Mathf.Sign(direction.x), 0);
                RaycastHit2D hit = Physics2D.BoxCast(origin, obstacleCheckSize, 0, castDir, 0.2f, groundLayer);
                if (hit.collider != null && IsGrounded())
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    animator.SetTrigger("jump");
                }
            }
            else
            {
                StopMoving();
            }
        }
        else
        {
            StopMoving();
        }
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("moving", false);
    }

    private Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closest = Mathf.Infinity;
        Transform target = null;

        foreach (GameObject p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < closest)
            {
                closest = dist;
                target = p.transform;
            }
        }

        return target;
    }

    private bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.6f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.2f, groundLayer);
        return hit.collider != null;
    }

    // Called by animation event (during skill animation)
    public void SummonLightning()
    {
        Debug.Log("LIGHTNING CAST!");
        if (lightningPrefab == null || player == null) return;

        Vector3 spawnPos = new Vector3(player.position.x, player.position.y - 0.5f, 0f);
        Instantiate(lightningPrefab, spawnPos, Quaternion.identity);
    }

    // Called by animation event at end of skill animation
    public void FinishSkill()
    {
        Debug.Log("SKILL FINISHED");
        isUsingSkill = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time - lastDamageTime > damageCooldown)
        {
            Health hp = collision.gameObject.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(meleeDamage);
                lastDamageTime = Time.time;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Vector2 castDir = Vector2.right * Mathf.Sign(transform.localScale.x);
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(origin + castDir * 0.2f, obstacleCheckSize);
    }
    // Called by animation event during attack animation
public void DamagePlayer()
{
    if (player != null && Vector2.Distance(transform.position, player.position) <= meleeRange)
    {
        Health hp = player.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(meleeDamage);
            Debug.Log("Monk damaged player with melee!");
        }
    }
}

}