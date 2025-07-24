using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpoint;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private UIManager uiManager;
    private Animator anim;
    private SkillManager skillManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindObjectOfType<UIManager>();
        anim = GetComponent<Animator>();
        skillManager = FindObjectOfType<SkillManager>();
        if (skillManager == null)
        {
            Debug.LogError("[PlayerRespawn] Could not find SkillManager in scene!");
        }
    }

    public void RespawnCheck()
    {
        if (currentCheckpoint == null) 
        {
            uiManager.GameOver();
            return;
        }

        transform.position = currentCheckpoint.position;

        playerHealth.Respawn();

        if (anim != null)
        {
            anim.SetBool("grounded", true);
            anim.SetBool("run", false);
            anim.Play("Idle", 0, 0f);
            anim.Update(0f);
            Debug.Log("[PlayerRespawn] Reset Animator to Idle state after respawn.");
        }
        else
        {
            Debug.LogError("[PlayerRespawn] Animator component not found!");
        }

        if (skillManager != null)
        {
            skillManager.UpdateNinjaInstance();
            Debug.Log("[PlayerRespawn] Updated SkillManager after respawn.");
        }
        else
        {
            Debug.LogError("[PlayerRespawn] SkillManager not found during respawn!");
        }

        Camera.main.GetComponent<CameraController>().MoveToNewRoom(currentCheckpoint.parent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Checkpoint")
        {
            currentCheckpoint = collision.transform;
            SoundManager.instance.PlaySound(checkpoint);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("activate");
        }
    }
}