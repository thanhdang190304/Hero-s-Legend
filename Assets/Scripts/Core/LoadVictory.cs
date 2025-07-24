using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadVictory : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Load profile
            string profileName = PlayerPrefs.GetString("ActiveProfile", null);
            if (string.IsNullOrEmpty(profileName))
            {
                Debug.LogError("No active profile found when completing level!");
                return;
            }

            PlayerProfile profile = SaveSystem.LoadProfile(profileName);
            if (profile == null)
            {
                Debug.LogError("Failed to load profile for: " + profileName);
                return;
            }
             SceneManager.LoadScene("_VictoryScreen_");
        }
    }
}