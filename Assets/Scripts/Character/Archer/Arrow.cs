using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Arrow : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float baseArcForce = 3f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 5f; // Max lifetime to prevent infinite travel

    private bool hit = false;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Vector3 initialScale;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform arrowVisual;

    private GameObject owner;
    private SkillManager skillManager; // Reference to SkillManager for piercing check
    private float spawnTime;

    public System.Action OnArrowHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        initialScale = transform.localScale;
        skillManager = FindObjectOfType<SkillManager>(); // Find SkillManager in the scene
        if (skillManager == null)
        {
            Debug.LogError("[Arrow] SkillManager not found in the scene!");
        }
    }

    private void OnEnable()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;
        rb.isKinematic = false;
        hit = false;
        boxCollider.enabled = true;
        transform.rotation = Quaternion.identity;
        transform.SetParent(null);
        spawnTime = Time.time; // Track when the arrow is spawned
    }

    private void Update()
    {
        if (!hit)
        {
            Vector2 velocity = rb.linearVelocity;

            if (velocity.sqrMagnitude > 0.1f)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // Deactivate after lifetime expires
            if (Time.time - spawnTime > lifetime)
            {
                Deactivate();
            }
        }
    }

    public void Launch(Vector2 direction, GameObject shooter, float speedMultiplier = 1f, float arcForce = 3f)
    {
        hit = false;
        owner = shooter;
        rb.linearVelocity = direction.normalized * (baseSpeed * speedMultiplier) + Vector2.up * arcForce;

        if (arrowVisual != null)
        {
            float x = Mathf.Abs(arrowVisual.localPosition.x);
            arrowVisual.localPosition = new Vector3(direction.x < 0 ? -x : x, arrowVisual.localPosition.y, arrowVisual.localPosition.z);

            var sr = arrowVisual.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipX = direction.x < 0;
        }

        transform.SetParent(null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit && !skillManager.IsPiercingActive) return;

        // Handle enemy damage if applicable
        if (collision.CompareTag("Enemy"))
        {
            SharkTank shark = collision.GetComponent<SharkTank>();
            if (shark != null)
            {
                shark.TakeDamage(damage, transform.position, owner);
            }
            else
            {
                Health enemyHealth = collision.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }

            // If piercing is active, continue flying through enemies
            if (skillManager.IsPiercingActive)
            {
                Debug.Log($"[Arrow] Piercing through enemy: {collision.gameObject.name}");
                return; // Skip stopping the arrow
            }
        }

        // Stop and deactivate on non-enemy collision (or enemy collision without piercing)
        hit = true;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        boxCollider.enabled = false;
        Invoke(nameof(Deactivate), 0.1f);
        Debug.Log($"[Arrow] Hit object: {collision.gameObject.name}, Tag: {collision.tag}, Layer: {LayerMask.LayerToName(collision.gameObject.layer)}");
    }

    private void Deactivate()
    {
        OnArrowHit?.Invoke();
        gameObject.SetActive(false);
    }
}