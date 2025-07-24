using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float resetTime = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private BoxCollider2D coll;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;

    private bool hasHit = false;
    private float lifetime;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (coll == null) coll = GetComponent<BoxCollider2D>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Fire on left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            FireArrow();
        }

        // Destroy arrow after lifetime expires
        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void FireArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - shootPoint.position).normalized;

        // Give upward and forward force for parabolic arc
        arrowRb.linearVelocity = new Vector2(direction.x * launchForce, launchForce);

        ArcherArrow arrowScript = arrow.GetComponent<ArcherArrow>();
        arrowScript.ResetLifetime();
    }

    public void ResetLifetime()
    {
        hasHit = false;
        lifetime = 0f;
        coll.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        hasHit = true;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        coll.enabled = false;

        if (anim != null)
        {
            anim.SetTrigger("explode");
        }
        else
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
