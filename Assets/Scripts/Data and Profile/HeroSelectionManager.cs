using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // Required for Button

public class HeroSelectionManager : MonoBehaviour
{
    private const string ActiveProfileKey = "ActiveProfile";

    [SerializeField] private TMP_InputField profileNameInput; // Assign in Inspector
    [SerializeField] private GameObject duplicateUsernameButton; // Assign the message button in Inspector
    [SerializeField] private TextMeshProUGUI errorMessageText; // Assign the error message text in Inspector
    [SerializeField] private Button backgroundClickArea; // Assign the background click area button in Inspector

    private int selectedSlotIndex;
    private string existingProfileName;
    private bool isPopupActive = false; // Track pop-up state

    public static bool CanProceed { get { return !FindObjectOfType<HeroSelectionManager>()?.isPopupActive ?? true; } }

    void Start()
    {
        // Retrieve the selected slot and profile name from ProfileSelection
        selectedSlotIndex = PlayerPrefs.GetInt("SelectedProfileSlot", -1);
        existingProfileName = PlayerPrefs.GetString("SelectedProfileName", "");

        // If loading an existing profile, pre-fill the input field
        if (!string.IsNullOrEmpty(existingProfileName) && SaveSystem.ProfileExists(existingProfileName))
        {
            PlayerProfile profile = SaveSystem.LoadProfile(existingProfileName);
            if (profile != null)
            {
                profileNameInput.text = profile.profileName;
                Debug.Log($"[HeroSelection] Pre-filled with existing profile: {existingProfileName}");
            }
            else
            {
                Debug.LogWarning($"[HeroSelection] Failed to load profile '{existingProfileName}', treating as new profile.");
                existingProfileName = ""; // Reset if invalid
                PlayerPrefs.SetString("SelectedProfileName", ""); // Clear invalid profile
                PlayerPrefs.Save();
            }
        }

        // Ensure the pop-up button and background are hidden initially
        if (duplicateUsernameButton != null)
        {
            duplicateUsernameButton.SetActive(false);
        }
        if (backgroundClickArea != null)
        {
            backgroundClickArea.gameObject.SetActive(false);
            backgroundClickArea.onClick.AddListener(HidePopup);
        }
        else
        {
            Debug.LogWarning("[HeroSelection] BackgroundClickArea not assigned, pop-up dismissal may not work.");
        }
    }

    // Called from each character button (pass 0,1,2 in the Button’s OnClick)
    public void SelectCharacter(int characterIndex)
    {
        string[] characterNames = { "Archer", "DragonWarrior", "Ninja" };

        if (characterIndex < 0 || characterIndex >= characterNames.Length)
        {
            Debug.LogError("Invalid character index");
            return;
        }

        string profileName = profileNameInput.text.Trim();
        if (string.IsNullOrEmpty(profileName))
        {
            ShowPopup("Please enter a profile name.");
            return; // Stay in scene
        }

        // Check if profile name is already taken (unless it's the existing profile)
        if (profileName != existingProfileName && SaveSystem.ProfileExists(profileName))
        {
            ShowPopup("No duplicated username");
            return; // Stay in scene
        }

        // If we reach here, the username is valid and unique
        string selectedCharacter = characterNames[characterIndex];

        // Create or update the profile
        PlayerProfile profile = new PlayerProfile(profileName, selectedCharacter);
        SaveSystem.SaveProfile(profile);

        // Associate the profile with the slot
        PlayerPrefs.SetString("ProfileSlot_" + selectedSlotIndex, profileName);
        PlayerPrefs.SetString(ActiveProfileKey, profileName);
        PlayerPrefs.Save();

        Debug.Log($"[HeroSelection] Created profile=\"{profileName}\" index={characterIndex} char={selectedCharacter} slot={selectedSlotIndex}");

        // Notify managers to reset their states
        if (MissionManager.instance != null)
        {
            MissionManager.instance.ForceResetMissions();
            Debug.Log("[HeroSelection] Notified MissionManager to reset missions.");
        }
        if (FindObjectOfType<CharacterSpawner>() != null)
        {
            CharacterSpawner spawner = FindObjectOfType<CharacterSpawner>();
            if (spawner != null)
            {
                spawner.SpawnCharacter();
                Debug.Log("[HeroSelection] Notified CharacterSpawner to spawn character.");
            }
        }
        else
        {
            Debug.Log("[HeroSelection] CharacterSpawner not found in this scene. Character will respawn on ActiveProfile change.");
        }

        // Load next scene only if username is valid
        SceneManager.LoadScene("_MainMenu_");
    }

    // Show the pop-up button with a custom message
    private void ShowPopup(string message)
    {
        if (duplicateUsernameButton != null && errorMessageText != null && backgroundClickArea != null)
        {
            errorMessageText.text = message;
            duplicateUsernameButton.SetActive(true);
            backgroundClickArea.gameObject.SetActive(true);
            isPopupActive = true; // Set pop-up active flag
            Time.timeScale = 1f; // Reset time scale to avoid external interference
            Debug.Log($"[HeroSelection] Showing pop-up: {message}, isPopupActive={isPopupActive}");
        }
        else
        {
            Debug.LogWarning("[HeroSelection] Pop-up button, error message text, or background click area not assigned in Inspector.");
        }
    }

    // Hide the pop-up button and background
    private void HidePopup()
    {
        if (duplicateUsernameButton != null && backgroundClickArea != null && isPopupActive)
        {
            duplicateUsernameButton.SetActive(false);
            backgroundClickArea.gameObject.SetActive(false);
            isPopupActive = false; // Reset pop-up active flag
            // Refocus the input field
            if (profileNameInput != null)
            {
                profileNameInput.ActivateInputField();
            }
            Debug.Log($"[HeroSelection] Pop-up dismissed, isPopupActive={isPopupActive}");
        }
    }
}