using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public Transform teleportTarget; // Drag your destination here
    public GameObject teleportEffectPrefab; // Drag your effect prefab here

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Play effect at the starting position
            if (teleportEffectPrefab != null)
            {
                Instantiate(teleportEffectPrefab, other.transform.position, Quaternion.identity);
            }

            // Teleport the player to the target position
            other.transform.position = teleportTarget.position;

            // Play effect at the destination position
            if (teleportEffectPrefab != null)
            {
                Instantiate(teleportEffectPrefab, teleportTarget.position, Quaternion.identity);
            }
        }
    }
}