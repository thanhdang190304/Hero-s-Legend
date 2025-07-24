using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons; // Assign 20 buttons in the Inspector (Level 1 to 20)

    private void Start()
    {
        if (levelButtons == null || levelButtons.Length != 20)
        {
            Debug.LogError("[LevelSelectionManager] Level buttons not assigned correctly! Expected 20 buttons.");
            return;
        }

        // Load the active profile
        string profileName = PlayerPrefs.GetString("ActiveProfile", null);
        if (string.IsNullOrEmpty(profileName))
        {
            Debug.LogError("[LevelSelectionManager] No active profile found!");
            DisableAllButtons();
            return;
        }

        PlayerProfile profile = SaveSystem.LoadProfile(profileName);
        if (profile == null)
        {
            Debug.LogError("[LevelSelectionManager] Failed to load profile for: " + profileName);
            DisableAllButtons();
            return;
        }

        // Enable buttons based on currentLevel (1-20)
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1; // 1 to 20
            bool isUnlocked = levelNumber <= profile.currentLevel;
            levelButtons[i].interactable = isUnlocked;
            Debug.Log($"[LevelSelectionManager] Level {levelNumber} button interactable: {isUnlocked} (currentLevel: {profile.currentLevel})");

            // Ensure the button's LevelLoader is set to the correct scene index (3-22)
            LevelLoader loader = levelButtons[i].GetComponent<LevelLoader>();
            if (loader != null)
            {
                int sceneIndex = i + 3; // Levels 1-20 map to scene indices 3-22
                // Note: We can't set private fields directly, so ensure levelIndex is set in the Inspector
                Debug.Log($"[LevelSelectionManager] Level {levelNumber} button set to load scene index: {sceneIndex}");
            }
            else
            {
                Debug.LogWarning($"[LevelSelectionManager] Level {levelNumber} button missing LevelLoader component!");
            }
        }
    }

    private void DisableAllButtons()
    {
        foreach (var button in levelButtons)
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }
}