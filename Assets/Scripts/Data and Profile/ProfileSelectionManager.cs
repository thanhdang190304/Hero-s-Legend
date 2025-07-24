using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProfileSelection : MonoBehaviour
{
    [SerializeField] private GameObject[] slotObjects; // Drag the three slot GameObjects here

    private const string ProfileSlotKey = "ProfileSlot_";
    private const int MaxProfiles = 3;

    void Start()
    {
        Debug.Log("ProfileSelection Start method called.");
        if (slotObjects == null || slotObjects.Length != MaxProfiles)
        {
            Debug.LogError($"Please assign exactly {MaxProfiles} slot GameObjects in the Inspector. Found: {slotObjects?.Length ?? 0}");
            return;
        }

        // Initialize UI for each profile slot
        for (int i = 0; i < MaxProfiles; i++)
        {
            int slotIndex = i; // Capture for lambda
            GameObject slot = slotObjects[i];
            if (slot == null)
            {
                Debug.LogError($"Slot {i} is not assigned in the slotObjects array.");
                continue;
            }

            // Dynamically construct text and button names based on slot index
            string textName = "Profile Status Text " + (i + 1);
            string loadButtonName = "Load or Create Button " + (i + 1);
            string deleteButtonName = "Delete " + (i + 1);

            Text usernameText = slot.transform.Find(textName)?.GetComponent<Text>();
            Button createLoadButton = slot.transform.Find(loadButtonName)?.GetComponent<Button>();
            Button deleteButton = slot.transform.Find(deleteButtonName)?.GetComponent<Button>();

            Debug.Log($"Checking slot {i}: usernameText={usernameText != null}, createLoadButton={createLoadButton != null}, deleteButton={deleteButton != null}");

            if (usernameText == null)
            {
                Debug.LogError($"Slot {i} is missing '{textName}' Text component.");
                continue;
            }
            if (createLoadButton == null)
            {
                Debug.LogError($"Slot {i} is missing '{loadButtonName}' Button component.");
                continue;
            }
            if (deleteButton == null)
            {
                Debug.LogError($"Slot {i} is missing '{deleteButtonName}' Button component.");
                continue;
            }

            // Find the Text component on the create/load button
            Text createLoadButtonText = createLoadButton.transform.Find("Text (Legacy)")?.GetComponent<Text>();
            if (createLoadButtonText == null)
            {
                Debug.LogError($"Slot {i} 'Load or Create Button' is missing 'Text (Legacy)' component.");
                continue;
            }

            string profileName = PlayerPrefs.GetString(ProfileSlotKey + slotIndex, "");

            if (!string.IsNullOrEmpty(profileName) && SaveSystem.ProfileExists(profileName))
            {
                PlayerProfile profile = SaveSystem.LoadProfile(profileName);
                // Display username, character, and gold in a single line
                usernameText.text = $"Name: {profile.profileName} Character: {profile.selectedCharacter} Gold: {profile.gold}";
                createLoadButtonText.text = "Load";
                deleteButton.gameObject.SetActive(true);
                Debug.Log($"[ProfileSelection] Slot {i} shows profile {profileName} with gold: {profile.gold}");
            }
            else
            {
                usernameText.text = "Empty";
                createLoadButtonText.text = "Create";
                deleteButton.gameObject.SetActive(false);
                Debug.Log($"[ProfileSelection] Slot {i} is empty.");
            }

            createLoadButton.onClick.RemoveAllListeners();
            deleteButton.onClick.RemoveAllListeners();

            createLoadButton.onClick.AddListener(() => OnCreateLoadButton(slotIndex));
            deleteButton.onClick.AddListener(() => OnDeleteButton(slotIndex));

            Debug.Log($"Slot {i} initialized. Profile: {(string.IsNullOrEmpty(profileName) ? "Empty" : profileName)}, Button listener added.");
        }
    }

    void OnCreateLoadButton(int slotIndex)
    {
        Debug.Log($"Load or Create button clicked for slot {slotIndex}");

        string profileName = PlayerPrefs.GetString(ProfileSlotKey + slotIndex, "");

        // Store the selected slot index and profile name
        PlayerPrefs.SetInt("SelectedProfileSlot", slotIndex);
        PlayerPrefs.SetString("SelectedProfileName", profileName);
        PlayerPrefs.Save();

        try
        {
            if (!string.IsNullOrEmpty(profileName) && SaveSystem.ProfileExists(profileName))
            {
                Debug.Log($"Loading existing profile '{profileName}' to _MainMenu_");
                PlayerPrefs.SetString("ActiveProfile", profileName); // Set active profile for _MainMenu_
                PlayerPrefs.Save();
                SceneManager.LoadScene("_MainMenu_");
            }
            else
            {
                Debug.Log($"No profile exists, loading _HeroSelect_ to create new profile");
                SceneManager.LoadScene("_HeroSelect_");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load scene: {e.Message}");
        }
    }

    void OnDeleteButton(int slotIndex)
    {
        Debug.Log($"Delete button clicked for slot {slotIndex}");

        string profileName = PlayerPrefs.GetString(ProfileSlotKey + slotIndex, "");
        if (!string.IsNullOrEmpty(profileName))
        {
            // Use SaveSystem to delete the profile, which handles read-only attribute
            SaveSystem.DeleteProfile(profileName);

            PlayerPrefs.DeleteKey(ProfileSlotKey + slotIndex);
            PlayerPrefs.Save();

            // Clear ActiveProfile if this was the active one
            string currentActiveProfile = PlayerPrefs.GetString("ActiveProfile", "");
            if (currentActiveProfile == profileName)
            {
                PlayerPrefs.SetString("ActiveProfile", "");
                PlayerPrefs.Save();
                Debug.Log($"[ProfileSelection] Cleared ActiveProfile (was {profileName}).");
            }

            // Notify managers to reset
            if (MissionManager.instance != null)
            {
                MissionManager.instance.ForceResetMissions();
                Debug.Log("[ProfileSelection] Notified MissionManager to reset missions.");
            }
            // Safely handle CharacterSpawner (might not be in this scene yet)
            if (FindObjectOfType<CharacterSpawner>() != null)
            {
                CharacterSpawner spawner = FindObjectOfType<CharacterSpawner>();
                if (spawner != null)
                {
                    spawner.SpawnCharacter();
                    Debug.Log("[ProfileSelection] Notified CharacterSpawner to spawn character.");
                }
            }
            else
            {
                Debug.Log("[ProfileSelection] CharacterSpawner not found in this scene. Character will respawn on ActiveProfile change.");
            }

            // Update UI for the slot
            GameObject slot = slotObjects[slotIndex];
            Text usernameText = slot.transform.Find("Profile Status Text " + (slotIndex + 1))?.GetComponent<Text>();
            Button createLoadButton = slot.transform.Find("Load or Create Button " + (slotIndex + 1))?.GetComponent<Button>();
            Text createLoadButtonText = createLoadButton?.transform.Find("Text (Legacy)")?.GetComponent<Text>();

            if (usernameText != null)
            {
                usernameText.text = "Empty";
            }
            if (createLoadButtonText != null)
            {
                createLoadButtonText.text = "Create";
            }
            Transform deleteTransform = slot.transform.Find("Delete " + (slotIndex + 1));
            if (deleteTransform != null)
            {
                deleteTransform.gameObject.SetActive(false);
            }
        }
    }
}