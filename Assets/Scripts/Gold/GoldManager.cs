using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    public int goldAmount; // Default gold amount
    public Text goldText; // Normal Unity UI Text (NOT TextMeshPro)

    private string currentProfileName;
    private string lastKnownProfileName; // Track the last known profile to detect changes

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GoldManager] Initialized as singleton.");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        currentProfileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
        lastKnownProfileName = currentProfileName;

        // Get selected character or use default
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Ninja");

        // Ensure profile exists
        if (!SaveSystem.ProfileExists(currentProfileName))
        {
            Debug.LogWarning($"[GoldManager] Profile {currentProfileName} not found, creating default profile.");
            PlayerProfile defaultProfile = new PlayerProfile(currentProfileName, selectedCharacter) { gold = 0 };
            defaultProfile.InitializeLevelStars();
            defaultProfile.InitializeMissionProgress();
            SaveSystem.SaveProfile(defaultProfile);
        }

        LoadGold();
        UpdateGoldUI();
        Debug.Log($"[GoldManager] Start - Loaded gold: {goldAmount} for profile: {currentProfileName}");
    }

    private void Update()
    {
        string newProfileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
        if (newProfileName != lastKnownProfileName)
        {
            Debug.Log($"[GoldManager] ActiveProfile changed from {lastKnownProfileName} to {newProfileName}. Reloading gold...");
            currentProfileName = newProfileName;
            lastKnownProfileName = newProfileName;
            LoadGold();
            UpdateGoldUI();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GoldManager] Scene loaded: {scene.name}");
        GameObject goldTextObject = GameObject.Find("GoldText");
        if (goldTextObject != null)
        {
            goldText = goldTextObject.GetComponent<Text>();
            LoadGold(); // Force reload gold from profile to ensure sync
            UpdateGoldUI();
            Debug.Log($"[GoldManager] Updated GoldText in {scene.name} to Gold: {goldAmount}");
        }
        else
        {
            Debug.LogWarning("[GoldManager] GoldText not found in scene: " + scene.name);
        }
    }

    public void AddGold(int amount)
    {
        goldAmount += amount;
        SaveGold();
        UpdateGoldUI();
        Debug.Log($"[GoldManager] Added {amount} gold. New total: {goldAmount}");
    }

    public void SpendGold(int amount)
    {
        if (goldAmount >= amount)
        {
            goldAmount -= amount;
            SaveGold();
            UpdateGoldUI();
            Debug.Log($"[GoldManager] Spent {amount} gold. New total: {goldAmount}");
        }
        else
        {
            Debug.LogWarning("[GoldManager] Not enough gold to spend!");
        }
    }

    public void SaveGold() // Changed from private to public
    {
        Debug.Log($"[GoldManager] Attempting to save gold: {goldAmount} for profile: {currentProfileName}");
        PlayerProfile profile = SaveSystem.LoadProfile(currentProfileName);
        if (profile != null)
        {
            profile.gold = goldAmount;
            SaveSystem.SaveProfile(profile);
            Debug.Log($"[GoldManager] Successfully saved gold: {goldAmount} to profile: {currentProfileName}");
        }
        else
        {
            Debug.LogWarning($"[GoldManager] No profile found for {currentProfileName}, creating new profile.");
            string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Ninja");
            PlayerProfile newProfile = new PlayerProfile(currentProfileName, selectedCharacter) { gold = goldAmount };
            newProfile.InitializeLevelStars();
            newProfile.InitializeMissionProgress();
            SaveSystem.SaveProfile(newProfile);
            Debug.Log($"[GoldManager] Created and saved new profile with gold: {goldAmount}");
        }
    }

    public void ReloadGoldFromProfile()
    {
        LoadGold();
        UpdateGoldUI();
        Debug.Log("[GoldManager] Reloaded gold from profile.");
    }

    private void LoadGold()
    {
        Debug.Log($"[GoldManager] Attempting to load gold for profile: {currentProfileName}");
        PlayerProfile profile = SaveSystem.LoadProfile(currentProfileName);
        if (profile != null)
        {
            goldAmount = profile.gold;
            Debug.Log($"[GoldManager] Loaded gold for profile {currentProfileName}: {goldAmount}");
        }
        else
        {
            Debug.LogWarning($"[GoldManager] No profile found for {currentProfileName}, initializing to 0 and creating profile.");
            goldAmount = 0;
            string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Ninja");
            PlayerProfile newProfile = new PlayerProfile(currentProfileName, selectedCharacter) { gold = goldAmount };
            newProfile.InitializeLevelStars();
            newProfile.InitializeMissionProgress();
            SaveSystem.SaveProfile(newProfile);
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + goldAmount.ToString();
            Debug.Log($"[GoldManager] Updated UI to Gold: {goldAmount}");
        }
        else
        {
            Debug.LogWarning("[GoldManager] goldText is null, UI not updated.");
        }
    }
}