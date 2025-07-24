using UnityEngine;

public class NinjaAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int attackDamage;

    [Header("Attack Box Collider")]
    [SerializeField] private float range; 
    [SerializeField] private float colliderDistance; 
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Enemy Layer")]
    [SerializeField] private LayerMask enemyLayer;

    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;

    // Public property to access attackDamage
    public int AttackDamage => attackDamage;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        var profileName = PlayerPrefs.GetString("ActiveProfile");
        var profile = SaveSystem.LoadProfile(profileName);

        if (profile != null)
        {
            attackCooldown = profile.attackCooldown;
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)) && cooldownTimer > attackCooldown)
        {
            Attack();
        }
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;
    }

    // Called by animation event
    private void DamageEnemy()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector2(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y), 
            0, Vector2.right * -transform.localScale.x, 0, enemyLayer);

        if (hit.transform != null && hit.transform.CompareTag("Enemy"))
        {
            // Check for SharkTank-specific logic
            SharkTank shark = hit.transform.GetComponent<SharkTank>();
            if (shark != null)
            {
                shark.TakeDamage(attackDamage, transform.position, gameObject);
            }
            else
            {
                Health enemyHealth = hit.transform.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z)
        );
    }
}