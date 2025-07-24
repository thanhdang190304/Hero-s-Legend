using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private Health playerHealth;

    private void Start()
    {
        TryFindPlayerHealth();
    }

    private void Update()
    {
        if (playerHealth == null)
        {
            TryFindPlayerHealth();
            return; // wait until we find the player
        }

        currentHealthBar.fillAmount = playerHealth.currentHealth / 10f;
    }

    private void TryFindPlayerHealth()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<Health>();

            if (playerHealth != null)
            {
                totalHealthBar.fillAmount = playerHealth.currentHealth / 10f;
            }
            else
            {
                Debug.LogWarning("[Healthbar] Player object found but missing Health component.");
            }
        }
    }
}
