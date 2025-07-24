using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    private bool triggered;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Player triggered checkpoint!");
            anim.SetTrigger("appearr");
        }
    }
}
