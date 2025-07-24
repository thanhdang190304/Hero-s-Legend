using UnityEngine;
using System.Collections;

public class SharkTank : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int damage = 1;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;

    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = new Color(0.44f, 0.15f, 0.15f, 1f); // Flash color
    [SerializeField] private float flashDuration = 0.1f; // Duration of each flash
    [SerializeField] private float totalFlashTime = 2f; // Total time for flash effect

    private float cooldownTimer = Mathf.Infinity;
    private int currentHealth;
    private Animator anim;
    private EnemyPatrol patrol;
    private Transform targetPlayer;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        patrol = GetComponentInParent<EnemyPatrol>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInFront() && !isAttacking)
        {
            if (patrol != null) patrol.enabled = false;

            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("attack");
                isAttacking = true;
                Debug.Log("Attack triggered!");
            }
        }
        else
        {
            if (patrol != null && !isAttacking) patrol.enabled = true;
        }
    }

    private bool PlayerInFront()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            direction,
            attackRange,
            playerLayer
        );

        if (hit.collider != null)
        {
            Debug.Log("BoxCast hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Player"))
            {
                targetPlayer = hit.transform;
                return true;
            }
        }

        targetPlayer = null;
        return false;
    }

    public void TakeDamage(int amount, Vector2 attackerPosition, GameObject attacker)
    {
        if (!attacker.CompareTag("Player"))
        {
            Debug.Log("Only player can damage the shark.");
            return;
        }

        Vector2 front = transform.right * transform.localScale.x;
        Vector2 toAttacker = ((Vector2)attackerPosition - (Vector2)transform.position).normalized;

        float dot = Vector2.Dot(front, toAttacker);

        if (dot < -0.5f) // Attacker is behind
        {
            currentHealth -= amount;
            anim.SetTrigger("hurt");
            StartCoroutine(FlashRed()); // Start the flash effect
            Debug.Log("Shark took damage from behind: " + amount);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("Attack from front ignored.");
        }
    }

    private void Die()
    {
        anim.SetTrigger("die");
        Debug.Log("Shark died");

        if (patrol != null) patrol.enabled = false;
        Destroy(gameObject, 1.5f);
    }

    private void OnAttackAnimationComplete()
    {
        isAttacking = false;
        anim.ResetTrigger("attack");
        Debug.Log("Attack animation finished");
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + direction * attackRange,
            boxCollider.bounds.size
        );
    }

    private IEnumerator FlashRed()
    {
        float flashInterval = totalFlashTime / 2;
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    // ✅ Call this from an animation event on the attack animation
    private void DamagePlayer()
{
    if (targetPlayer != null)
    {
        Health playerHealth = targetPlayer.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Player damaged for: " + damage);
        }
    }
}

}
