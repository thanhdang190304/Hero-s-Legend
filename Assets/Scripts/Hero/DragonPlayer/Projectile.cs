using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject burningParticlePrefab; // Assign in Inspector
    private float direction;
    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private Vector2 playerPosition;
    private GameObject player;
    private SkillManager skillManager; // Reference to SkillManager

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        skillManager = FindObjectOfType<SkillManager>();
        if (skillManager == null)
        {
            Debug.LogError("[Projectile] SkillManager not found in the scene!");
        }
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        if (collision.tag == "Enemy")
        {
            SharkTank shark = collision.GetComponent<SharkTank>();
            if (shark != null)
            {
                shark.TakeDamage(1, playerPosition, player);
                if (skillManager.IsBurningEffectActive)
                {
                    StartCoroutine(ApplyBurningEffect(shark));
                }
            }
            else
            {
                Health enemyHealth = collision.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(1);
                    if (skillManager.IsBurningEffectActive)
                    {
                        StartCoroutine(ApplyBurningEffect(enemyHealth));
                    }
                }
            }
        }
    }

    private IEnumerator ApplyBurningEffect(object enemy)
    {
        // Determine the type of enemy (SharkTank or Health component)
        SharkTank shark = enemy as SharkTank;
        Health enemyHealth = enemy as Health;
        GameObject enemyObj = (shark != null) ? shark.gameObject : (enemyHealth != null) ? enemyHealth.gameObject : null;

        if (enemyObj == null)
        {
            Debug.LogWarning("[Projectile] Invalid enemy for burning effect");
            yield break;
        }

        // Check if the enemy is already burning (using a tag or component)
        BurningEffect existingBurn = enemyObj.GetComponent<BurningEffect>();
        if (existingBurn != null)
        {
            Debug.Log($"[Projectile] Enemy {enemyObj.name} is already burning, skipping additional burn");
            yield break;
        }

        // Add BurningEffect component to track burning state
        BurningEffect burnEffect = enemyObj.AddComponent<BurningEffect>();
        burnEffect.Initialize(1f, 2f, burningParticlePrefab, playerPosition, player);

        Debug.Log($"[Projectile] Applied burning effect to {enemyObj.name}");
    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    public void SetPlayerDetails(Vector2 _playerPosition, GameObject _player)
    {
        playerPosition = _playerPosition;
        player = _player;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}

// Helper component to manage burning effect on enemies
public class BurningEffect : MonoBehaviour
{
    private float damagePerSecond;
    private float duration;
    private GameObject burningParticleInstance;
    private Vector2 playerPosition;
    private GameObject player;

    public void Initialize(float _damagePerSecond, float _duration, GameObject particlePrefab, Vector2 _playerPosition, GameObject _player)
    {
        damagePerSecond = _damagePerSecond;
        duration = _duration;
        playerPosition = _playerPosition;
        player = _player;

        // Instantiate burning particle effect if assigned
        if (particlePrefab != null)
        {
            burningParticleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity, transform);
            burningParticleInstance.transform.localPosition = Vector3.zero;
        }

        StartCoroutine(Burn());
    }

    private IEnumerator Burn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Apply damage every second
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;

            SharkTank shark = GetComponent<SharkTank>();
            if (shark != null)
            {
                shark.TakeDamage((int)damagePerSecond, playerPosition, player);
            }
            else
            {
                Health enemyHealth = GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((int)damagePerSecond);
                }
            }

            Debug.Log($"[BurningEffect] Dealt {damagePerSecond} burn damage to {gameObject.name}");
        }

        // Clean up
        if (burningParticleInstance != null)
        {
            Destroy(burningParticleInstance);
        }
        Destroy(this);
    }
}