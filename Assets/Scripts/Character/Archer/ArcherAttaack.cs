using UnityEngine;

public class ArcherAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform arrowPoint;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private AudioClip arrowSound;

    [Header("Charge Settings")]
    [SerializeField] private float maxChargeTime = 2f; // Maximum time to charge (e.g., 2 seconds)
    [SerializeField] private float minSpeedMultiplier = 1f; // Minimum speed multiplier (default)
    [SerializeField] private float maxSpeedMultiplier = 3f; // Maximum speed multiplier
    [SerializeField] private float minArcForce = 3f; // Minimum arc force
    [SerializeField] private float maxArcForce = 6f; // Maximum arc force

    private Animator anim;
    private ArcherMovement archerMovement;
    private float cooldownTimer = Mathf.Infinity;
    private float chargeTimer = 0f;
    private bool isCharging = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        archerMovement = GetComponent<ArcherMovement>();
        
        var profileName = PlayerPrefs.GetString("ActiveProfile");
        var profile = SaveSystem.LoadProfile(profileName);

        if (profile != null)
        {
            attackCooldown = profile.attackCooldown;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cooldownTimer > attackCooldown && archerMovement.canAttack())
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime); // Cap at maxChargeTime
        }

        if (Input.GetMouseButtonUp(0) && isCharging && archerMovement.canAttack())
        {
            Attack();
            isCharging = false;
        }

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        SoundManager.instance.PlaySound(arrowSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        int arrowIndex = FindArrow();
        GameObject arrow = arrows[arrowIndex];
        arrow.SetActive(true);
        arrow.transform.SetParent(null); 
        arrow.transform.position = arrowPoint.position;

        Vector3 targetPos = GetMouseWorldPosition();
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        
        // Calculate charge factor (0 to 1 based on charge time)
        float chargeFactor = chargeTimer / maxChargeTime;
        float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, chargeFactor);
        float arcForce = Mathf.Lerp(minArcForce, maxArcForce, chargeFactor);

        // Pass in shooter reference and charged power
        arrowScript.Launch((targetPos - arrowPoint.position).normalized, gameObject, speedMultiplier, arcForce);
    }

    private int FindArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            return hit.point;
        else
            return ray.GetPoint(30f);
    }
}