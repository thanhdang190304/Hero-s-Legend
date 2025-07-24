using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletionTrigger : MonoBehaviour
{
    [SerializeField] private LevelTimer levelTimer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[LevelCompletionTrigger] Player touched flag in scene: {SceneManager.GetActiveScene().name}, Index: {SceneManager.GetActiveScene().buildIndex}");

            if (levelTimer == null)
            {
                Debug.LogError("[LevelCompletionTrigger] LevelTimer not assigned!");
                return;
            }

            levelTimer.StopTimer();
            Debug.Log($"[LevelCompletionTrigger] Timer stopped. Current time: {levelTimer.GetCurrentTime()}");

            float timeRemaining = levelTimer.GetCurrentTime();
            float initialTime = levelTimer.GetTimeLimit();
            if (initialTime <= 0)
            {
                Debug.LogError("[LevelCompletionTrigger] Initial time is zero or negative!");
                return;
            }
            float timePercentage = (timeRemaining / initialTime) * 100;
            int stars = CalculateStars(timePercentage);
            Debug.Log($"[LevelCompletionTrigger] Time remaining: {timeRemaining}/{initialTime}, Percentage: {timePercentage}%, Stars: {stars}");

            int levelIndex = SceneManager.GetActiveScene().buildIndex - 3;
            Debug.Log($"[LevelCompletionTrigger] Current scene index: {SceneManager.GetActiveScene().buildIndex}, Calculated levelIndex: {levelIndex}");
            if (levelIndex < 0 || levelIndex >= 20)
            {
                Debug.LogError($"[LevelCompletionTrigger] Invalid level index: {levelIndex}. Expected range: 0-19 (build indices 3-22).");
                return;
            }

            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.EndTrackingLevel(levelIndex);
                Debug.Log($"[LevelCompletionTrigger] Ended damage tracking for level {levelIndex}, damage taken: {playerHealth.DidTakeDamageInLevel(levelIndex)}");
            }
            else
            {
                Debug.LogError("[LevelCompletionTrigger] Player Health component not found!");
            }

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

            profile.levelStars[levelIndex] = stars;
<<<<<<< Updated upstream
            profile.upgradePoints += 0.1f;
            Debug.Log($"[LevelCompletion] {profileName} rewarded 0.1 points. New total: {profile.upgradePoints}");
=======
            profile.upgradePoints += 50f;
            Debug.Log($"[LevelCompletion] {profileName} rewarded 50 points. New total: {profile.upgradePoints}");
>>>>>>> Stashed changes

            int completedLevelNumber = levelIndex + 1;
            if (completedLevelNumber == profile.currentLevel)
            {
                profile.currentLevel = Mathf.Min(profile.currentLevel + 1, 21);
                Debug.Log($"[LevelCompletion] Unlocked next level. New currentLevel: {profile.currentLevel}");
            }
            else
            {
                Debug.Log($"[LevelCompletion] Level {completedLevelNumber} completed, but currentLevel ({profile.currentLevel}) not updated (replay or earlier level).");
            }

            SaveSystem.SaveProfile(profile);
            Debug.Log($"[LevelCompletion] {profileName} earned {stars} stars on Level {levelIndex + 1}. Total upgradePoints: {profile.upgradePoints}");

            PlayerPrefs.SetInt("StarsEarned", stars);
            PlayerPrefs.SetInt("LevelCompleted", levelIndex + 1);
            PlayerPrefs.SetFloat("TimeLimit", initialTime);
            PlayerPrefs.Save();
            Debug.Log("[LevelCompletionTrigger] PlayerPrefs saved: StarsEarned=" + stars + ", LevelCompleted=" + (levelIndex + 1) + ", TimeLimit=" + initialTime);

            Debug.Log("[LevelCompletionTrigger] Loading _StarReward_ scene (index 23)...");
            SceneManager.LoadScene("_StarReward_");
        }
    }

    private int CalculateStars(float percentage)
    {
        if (percentage >= 70) return 3;
        else if (percentage >= 40) return 2;
        else if (percentage >= 10) return 1;
        else return 0;
    }
}