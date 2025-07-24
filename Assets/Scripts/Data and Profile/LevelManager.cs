using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelTimer levelTimer; // Assign in Inspector

    public void FinishLevel()
    {
        if (levelTimer == null)
        {
            Debug.LogError("[LevelManager] LevelTimer not assigned!");
            return;
        }

        // Stop the timer
        levelTimer.StopTimer();

        // Calculate percentage of time remaining
        float timeRemaining = levelTimer.GetCurrentTime();
        float initialTime = levelTimer.GetTimeLimit();
        float timePercentage = (timeRemaining / initialTime) * 100;
        int stars = CalculateStars(timePercentage);

        // Get the current level index (scenes are ordered: 0 for _ProfileSelection, 1 for _HeroSelect, 2 for _MainMenu, 3-22 for levels 1-20)
        int levelIndex = SceneManager.GetActiveScene().buildIndex - 3; // Adjusted offset for levels starting at index 3
        if (levelIndex < 0 || levelIndex >= 20)
        {
            Debug.LogError($"[LevelManager] Invalid level index: {levelIndex}");
            return;
        }

        // Load the active profile and save stars
        string activeProfileName = PlayerPrefs.GetString("ActiveProfile", "");
        if (!string.IsNullOrEmpty(activeProfileName) && SaveSystem.ProfileExists(activeProfileName))
        {
            PlayerProfile profile = SaveSystem.LoadProfile(activeProfileName);
            profile.levelStars[levelIndex] = stars; // Save stars for this level
            SaveSystem.SaveProfile(profile);
        }
        else
        {
            Debug.LogError("[LevelManager] No active profile found!");
        }

        // Store stars for _StarReward_ scene
        PlayerPrefs.SetInt("StarsEarned", stars);
        PlayerPrefs.SetInt("LevelCompleted", levelIndex + 1); // Store level number (1-20)
        PlayerPrefs.Save();

        SceneManager.LoadScene("_StarReward_");
    }

    private int CalculateStars(float percentage)
    {
        if (percentage >= 70) return 3;
        else if (percentage >= 40) return 2;
        else if (percentage >= 10) return 1;
        else return 0;
    }
}