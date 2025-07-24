using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StarsRewardDisplay : MonoBehaviour
{
    [SerializeField] private Text starsText; // Text component to display the list of stars
    [SerializeField] private Button backButton; // Button to return to the main menu

    // Reward display buttons (to show criteria)
    [SerializeField] private Button tenStarRewardButton;
    [SerializeField] private Button thirtyStarRewardButton;
    [SerializeField] private Button fortyStarRewardButton;

    // Claim buttons
    [SerializeField] private Button claimTenStarRewardButton;
    [SerializeField] private Button claimThirtyStarRewardButton;
    [SerializeField] private Button claimFortyStarRewardButton;

    private PlayerProfile profile;

    void Start()
    {
        // Load the active profile
        string profileName = PlayerPrefs.GetString("ActiveProfile", null);
        if (string.IsNullOrEmpty(profileName))
        {
            Debug.LogError("[StarsRewardDisplay] No active profile found!");
            if (starsText != null)
            {
                starsText.text = "No profile selected!";
            }
            return;
        }

        profile = SaveSystem.LoadProfile(profileName);
        if (profile == null)
        {
            Debug.LogError("[StarsRewardDisplay] Failed to load profile for: " + profileName);
            if (starsText != null)
            {
                starsText.text = "Failed to load profile!";
            }
            return;
        }

        // Ensure levelStars is initialized
        if (profile.levelStars == null || profile.levelStars.Length != 20)
        {
            Debug.LogWarning("[StarsRewardDisplay] levelStars array is invalid, initializing...");
            profile.InitializeLevelStars();
            SaveSystem.SaveProfile(profile);
        }

        // Build the list of stars for each level (1 to 20)
        string starsList = "Stars Earned:\n";
        int totalStars = 0;
        for (int i = 0; i < 20; i++)
        {
            int levelNumber = i + 1; // Level 1 to 20
            int starsEarned = profile.levelStars[i];
            totalStars += starsEarned;
            starsList += $"Level {levelNumber} - {starsEarned}/3 stars\n";
        }

        // Update the Text component
        if (starsText != null)
        {
            starsText.text = starsList;
        }
        else
        {
            Debug.LogError("[StarsRewardDisplay] StarsText not assigned!");
        }

        // Set up reward buttons with criteria
        if (tenStarRewardButton != null)
        {
            Text buttonText = tenStarRewardButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "10 Stars - 20 Gold";
        }
        if (thirtyStarRewardButton != null)
        {
            Text buttonText = thirtyStarRewardButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "30 Stars - 50 Gold";
        }
        if (fortyStarRewardButton != null)
        {
            Text buttonText = fortyStarRewardButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "40 Stars - 100 Gold";
        }

        // Set up claim buttons with "Claim" text
        if (claimTenStarRewardButton != null)
        {
            Text buttonText = claimTenStarRewardButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "Claim";
            claimTenStarRewardButton.interactable = totalStars >= 10 && !profile.tenStarRewardClaimed;
            claimTenStarRewardButton.onClick.AddListener(() => ClaimReward(10, 20, ref profile.tenStarRewardClaimed));
        }
        if (claimThirtyStarRewardButton != null)
        {
            Text buttonText = claimThirtyStarRewardButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "Claim";
            claimThirtyStarRewardButton.interactable = totalStars >= 30 && !profile.thirtyStarRewardClaimed;
            claimThirtyStarRewardButton.onClick.AddListener(() => ClaimReward(30, 50, ref profile.thirtyStarRewardClaimed));
        }
        if (claimFortyStarRewardButton != null)
        {
            Text buttonText = claimFortyStarRewardButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "Claim";
            claimFortyStarRewardButton.interactable = totalStars >= 40 && !profile.fortyStarRewardClaimed;
            claimFortyStarRewardButton.onClick.AddListener(() => ClaimReward(40, 100, ref profile.fortyStarRewardClaimed));
        }

        // Set up the back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBack);
        }
        else
        {
            Debug.LogError("[StarsRewardDisplay] BackButton not assigned!");
        }
    }

    void ClaimReward(int requiredStars, int goldReward, ref bool claimedFlag)
    {
        if (profile == null || GoldManager.Instance == null) return;

        // Use GoldManager to add gold
        GoldManager.Instance.AddGold(goldReward);

        // Mark as claimed (this will sync with PlayerProfile via GoldManager.SaveGold())
        claimedFlag = true;

        // Update claim button interactability
        if (requiredStars == 10 && claimTenStarRewardButton != null)
        {
            claimTenStarRewardButton.interactable = false;
        }
        else if (requiredStars == 30 && claimThirtyStarRewardButton != null)
        {
            claimThirtyStarRewardButton.interactable = false;
        }
        else if (requiredStars == 40 && claimFortyStarRewardButton != null)
        {
            claimFortyStarRewardButton.interactable = false;
        }

        Debug.Log($"[StarsRewardDisplay] Claimed {goldReward} gold for {requiredStars} stars. New gold: {GoldManager.Instance.goldAmount}");
    }

    void OnBack()
    {
        SceneManager.LoadScene("_MainMenu_");
    }
}