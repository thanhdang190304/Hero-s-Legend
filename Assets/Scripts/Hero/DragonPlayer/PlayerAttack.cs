using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown ;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;
    [SerializeField] private Transform fireballHolder; // Drag this in inspector (child of Player in prefab)

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // Detach FireballHolder at runtime to prevent it from following player flips
        if (fireballHolder != null)
            fireballHolder.SetParent(null);

        var profileName = PlayerPrefs.GetString("ActiveProfile");
        var profile = SaveSystem.LoadProfile(profileName);

        if (profile != null)
        {
            attackCooldown = profile.attackCooldown;
        }
    }

    private void Update()
    {
        if ((Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Return)) && cooldownTimer > attackCooldown && playerMovement.canAttack())
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        SoundManager.instance.PlaySound(fireballSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        GameObject fireball = fireballs[FindFireball()];
        fireball.transform.position = firePoint.position;

        // Detach the fireball so it's independent
        fireball.transform.SetParent(null);

        float direction = Mathf.Sign(transform.localScale.x);
        fireball.GetComponent<Projectile>().SetDirection(direction);
        fireball.GetComponent<Projectile>().SetPlayerDetails(transform.position, gameObject);
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}