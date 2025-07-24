using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("Stat Upgrade UI")]
    public Text speedText, jumpText, cooldownText, pointsText;

    [Header("Unlock UI")]
    public Button doubleJumpButton, wallJumpButton;
    public Text doubleJumpStatusText;
    public Text wallJumpStatusText;

    [Header("Upgrade Buttons")]
    public Button upgradeSpeedButton, upgradeJumpButton, upgradeCooldownButton;

    [Header("Buy Points")]
    public Button buyPointsButton;
    public int goldPerPointUnit = 5;
    public float pointsPerPurchaseUnit = 0.1f;

    [Header("Cost")]
    public int doubleJumpCost = 1;
    public int wallJumpCost = 300;

    private PlayerProfile profile;

    void Start()
    {
        string profileName = PlayerPrefs.GetString("ActiveProfile", null);
        if (string.IsNullOrEmpty(profileName))
        {
            Debug.LogError("No active profile in PlayerPrefs.");
            return;
        }

        profile = SaveSystem.LoadProfile(profileName);
        if (profile == null)
        {
            Debug.LogError("Profile could not be loaded.");
            return;
        }

        // Attach upgrade listeners
        upgradeSpeedButton.onClick.AddListener(UpgradeSpeed);
        upgradeJumpButton.onClick.AddListener(UpgradeJump);
        upgradeCooldownButton.onClick.AddListener(UpgradeCooldown);

        // Attach unlock listeners
        doubleJumpButton.onClick.AddListener(UnlockDoubleJump);
        wallJumpButton.onClick.AddListener(UnlockWallJump);

        // Buy points listener
        if (buyPointsButton != null)
            buyPointsButton.onClick.AddListener(BuyUpgradePoints);

        UpdateUI();
    }

    void UpgradeSpeed()
    {
        if (profile.upgradePoints >= 0.1f)
        {
            profile.speed += 0.1f;
            profile.upgradePoints -= 0.1f;
            SaveSystem.SaveProfile(profile);
            UpdateUI();
        }
    }

    void UpgradeJump()
    {
        if (profile.upgradePoints >= 0.1f)
        {
            profile.jumpPower += 0.1f;
            profile.upgradePoints -= 0.1f;
            SaveSystem.SaveProfile(profile);
            UpdateUI();
        }
    }

    void UpgradeCooldown()
    {
        if (profile.upgradePoints >= 0.1f)
        {
            profile.attackCooldown -= 0.1f;
            if (profile.attackCooldown < 0.1f)
                profile.attackCooldown = 0.1f;

            profile.upgradePoints -= 0.1f;
            SaveSystem.SaveProfile(profile);
            UpdateUI();
        }
    }

    void UnlockDoubleJump()
    {
        if (profile.doubleJumpUnlocked)
        {
            doubleJumpStatusText.text = "Unlocked";
            return;
        }

        if (GoldManager.Instance.goldAmount >= doubleJumpCost)
        {
            GoldManager.Instance.SpendGold(doubleJumpCost);
            profile.doubleJumpUnlocked = true;
            profile.extraJumps = 1;
            profile.gold = GoldManager.Instance.goldAmount; // Sync gold with GoldManager
            SaveSystem.SaveProfile(profile);
            doubleJumpStatusText.text = "Unlocked!";
            UpdateUI();
        }
        else
        {
            doubleJumpStatusText.text = "Not enough gold!";
        }
    }

    void UnlockWallJump()
    {
        if (profile.wallJumpUnlocked)
        {
            wallJumpStatusText.text = "Unlocked";
            return;
        }

        if (GoldManager.Instance.goldAmount >= wallJumpCost)
        {
            GoldManager.Instance.SpendGold(wallJumpCost);
            profile.wallJumpUnlocked = true;
            profile.wallJumpX = 1500;
            profile.wallJumpY = 750;
            profile.gold = GoldManager.Instance.goldAmount; // Sync gold with GoldManager
            SaveSystem.SaveProfile(profile);
            wallJumpStatusText.text = "Unlocked!";
            UpdateUI();
        }
        else
        {
            wallJumpStatusText.text = "Not enough gold!";
        }
    }

    void BuyUpgradePoints()
    {
        int requiredGold = Mathf.RoundToInt(goldPerPointUnit);
        if (GoldManager.Instance.goldAmount >= requiredGold)
        {
            GoldManager.Instance.SpendGold(requiredGold); // Deduct gold and save to profile
            profile.upgradePoints += pointsPerPurchaseUnit; // Increase points by 0.1
            profile.gold = GoldManager.Instance.goldAmount; // Sync gold with GoldManager
            SaveSystem.SaveProfile(profile); // Save profile with updated points and gold
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough gold to buy upgrade points.");
        }
    }

    void UpdateUI()
    {
        speedText.text = $"Speed: {profile.speed:F1}";
        jumpText.text = $"Jump: {profile.jumpPower:F1}";
        cooldownText.text = $"Cooldown: {profile.attackCooldown:F1}";
        pointsText.text = $"Points: {profile.upgradePoints:F1}";

        doubleJumpStatusText.text = profile.doubleJumpUnlocked ? "Unlocked" : $"Buy for {doubleJumpCost} Gold";
        wallJumpStatusText.text = profile.wallJumpUnlocked ? "Unlocked" : $"Buy for {wallJumpCost} Gold";
    }
}