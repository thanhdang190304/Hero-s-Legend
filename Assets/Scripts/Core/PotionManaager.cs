using UnityEngine;
using UnityEngine.UI; // For UI Image
using System.Collections;

public class PotionManager : MonoBehaviour
{
    public int healthPotionCount = 0;
    public int timerPotionCount = 0;
    public int invulnerabilityPotionCount = 0;

    public int maxPotionStack = 3;
    public int maxInvulnerabilityPotionStack = 1;

    private Health playerHealth;
    private LevelTimer levelTimer;
    private float potionCooldown = 3f; // 3-second cooldown for each potion
    private float lastHealthPotionUseTime = -3f; // Track last use time for health potion
    private float lastTimerPotionUseTime = -3f;  // Track last use time for timer potion
    private float lastInvulnerabilityPotionUseTime = -3f; // Track last use time for invulnerability potion

    [SerializeField] private Image healthPotionCooldownImage; // Cooldown image for health potion
    [SerializeField] private Image timerPotionCooldownImage;  // Cooldown image for timer potion
    [SerializeField] private Image invulnerabilityPotionCooldownImage; // Cooldown image for invulnerability potion

    private void Start()
    {
        StartCoroutine(WaitForPlayerAndLoadProfile());
    }

    private IEnumerator WaitForPlayerAndLoadProfile()
    {
        // Wait until the Player exists
        while (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }
            yield return null; // wait 1 frame
        }

        levelTimer = FindObjectOfType<LevelTimer>();

        if (levelTimer == null)
        {
            Debug.LogWarning("[PotionManager] Could not find LevelTimer in scene.");
        }

        // Load potion counts from profile
        string profileName = PlayerPrefs.GetString("ActiveProfile");
        PlayerProfile profile = SaveSystem.LoadProfile(profileName);

        if (profile != null)
        {
            healthPotionCount = profile.healthPotionCount;
            timerPotionCount = profile.timerPotionCount;
            invulnerabilityPotionCount = profile.invulnerabilityPotionCount;
        }

        Debug.Log("[PotionManager] Initialized.");
    }

    private bool IsHealthPotionCooldownOver()
    {
        return Time.time >= lastHealthPotionUseTime + potionCooldown;
    }

    private bool IsTimerPotionCooldownOver()
    {
        return Time.time >= lastTimerPotionUseTime + potionCooldown;
    }

    private bool IsInvulnerabilityPotionCooldownOver()
    {
        return Time.time >= lastInvulnerabilityPotionUseTime + potionCooldown;
    }

    private float GetHealthPotionCooldownProgress()
    {
        if (IsHealthPotionCooldownOver())
        {
            return 0f; // Cooldown is over, no fill
        }
        float timeSinceLastUse = Time.time - lastHealthPotionUseTime;
        return 1f - (timeSinceLastUse / potionCooldown); // Returns 1 to 0 as cooldown progresses
    }

    private float GetTimerPotionCooldownProgress()
    {
        if (IsTimerPotionCooldownOver())
        {
            return 0f; // Cooldown is over, no fill
        }
        float timeSinceLastUse = Time.time - lastTimerPotionUseTime;
        return 1f - (timeSinceLastUse / potionCooldown); // Returns 1 to 0 as cooldown progresses
    }

    private float GetInvulnerabilityPotionCooldownProgress()
    {
        if (IsInvulnerabilityPotionCooldownOver())
        {
            return 0f; // Cooldown is over, no fill
        }
        float timeSinceLastUse = Time.time - lastInvulnerabilityPotionUseTime;
        return 1f - (timeSinceLastUse / potionCooldown); // Returns 1 to 0 as cooldown progresses
    }

    private void Update()
    {
        // Update cooldown UI for all potion images
        if (healthPotionCooldownImage != null)
        {
            healthPotionCooldownImage.fillAmount = healthPotionCount == 0 ? 1f : GetHealthPotionCooldownProgress();
        }
        if (timerPotionCooldownImage != null)
        {
            timerPotionCooldownImage.fillAmount = timerPotionCount == 0 ? 1f : GetTimerPotionCooldownProgress();
        }
        if (invulnerabilityPotionCooldownImage != null)
        {
            invulnerabilityPotionCooldownImage.fillAmount = invulnerabilityPotionCount == 0 ? 1f : GetInvulnerabilityPotionCooldownProgress();
        }

        // Check for key presses to use potions (reordered to match left-to-right: yellow, red, clock)
        if (Input.GetKeyDown(KeyCode.Alpha1) && healthPotionCount > 0 && IsHealthPotionCooldownOver()) // Key 1 for health potion
        {
            UseHealthPotion();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && timerPotionCount > 0 && IsTimerPotionCooldownOver()) // Key 2 for timer potion
        {
            UseTimerPotion();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && invulnerabilityPotionCount > 0 && IsInvulnerabilityPotionCooldownOver()) // Key 3 for invulnerability potion
        {
            UseInvulnerabilityPotion();
        }
    }

    public void UseHealthPotion()
    {
        if (healthPotionCount > 0 && playerHealth != null && IsHealthPotionCooldownOver())
        {
            healthPotionCount--;
            playerHealth.AddHealth(1f); // Instantly restore 1 health
            lastHealthPotionUseTime = Time.time; // Update health potion cooldown timer
            Debug.Log("[PotionManager] Used Health Potion. +1 health. Health potion cooldown started.");
            UpdateProfile();
        }
        else
        {
            Debug.Log("[PotionManager] Cannot use health potion — none left, missing player health, or cooldown active.");
        }
    }

    public void UseTimerPotion()
    {
        if (timerPotionCount > 0 && levelTimer != null && IsTimerPotionCooldownOver())
        {
            timerPotionCount--;
            levelTimer.AddTime(15f); // Instantly add 15 seconds
            lastTimerPotionUseTime = Time.time; // Update timer potion cooldown timer
            Debug.Log("[PotionManager] Used Timer Potion. +15 seconds. Timer potion cooldown started.");
            UpdateProfile();
        }
        else
        {
            Debug.Log("[PotionManager] Cannot use timer potion — none left, missing timer, or cooldown active.");
        }
    }

    public void UseInvulnerabilityPotion()
    {
        if (invulnerabilityPotionCount > 0 && playerHealth != null && IsInvulnerabilityPotionCooldownOver())
        {
            invulnerabilityPotionCount--;
            StartCoroutine(ApplyInvulnerability());
            lastInvulnerabilityPotionUseTime = Time.time; // Update invulnerability potion cooldown timer
            Debug.Log("[PotionManager] Used Invulnerability Potion. Player is invulnerable for 10 seconds. Invulnerability potion cooldown started.");
            UpdateProfile();
        }
        else
        {
            Debug.Log("[PotionManager] Cannot use invulnerability potion — none left, missing player health, or cooldown active.");
        }
    }

    private IEnumerator ApplyInvulnerability()
    {
        playerHealth.SetInvulnerable(true);
        yield return new WaitForSeconds(10f);
        playerHealth.SetInvulnerable(false);
    }

    public bool CanAddHealthPotion() => healthPotionCount < maxPotionStack;
    public bool CanAddTimerPotion() => timerPotionCount < maxPotionStack;
    public bool CanAddInvulnerabilityPotion() => invulnerabilityPotionCount < maxInvulnerabilityPotionStack;

    public void AddHealthPotion()
    {
        if (CanAddHealthPotion())
        {
            healthPotionCount++;
            UpdateProfile();
        }
    }

    public void AddTimerPotion()
    {
        if (CanAddTimerPotion())
        {
            timerPotionCount++;
            UpdateProfile();
        }
    }

    public void AddInvulnerabilityPotion()
    {
        if (CanAddInvulnerabilityPotion())
        {
            invulnerabilityPotionCount++;
            UpdateProfile();
        }
    }

    private void UpdateProfile()
    {
        string profileName = PlayerPrefs.GetString("ActiveProfile");
        PlayerProfile profile = SaveSystem.LoadProfile(profileName);

        if (profile != null)
        {
            profile.healthPotionCount = healthPotionCount;
            profile.timerPotionCount = timerPotionCount;
            profile.invulnerabilityPotionCount = invulnerabilityPotionCount;
            SaveSystem.SaveProfile(profile);
        }
    }
}