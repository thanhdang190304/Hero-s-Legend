using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public Text defenseValueText;
    public Text armor1StatusText;
    public Text armor2StatusText;
    public Text armor3StatusText;

    public Text healthPotionStatusText;
    public Text timerPotionStatusText;
    public Text invulnPotionStatusText;

    public Text healthPotionNumberText;
    public Text timerPotionNumberText;
    public Text invulnPotionNumberText;

    public Button armor1Button;
    public Button armor2Button;
    public Button armor3Button;

    public Button buyHealthPotionButton;
    public Button buyTimerPotionButton;
    public Button buyInvulnPotionButton;

    [Header("Armor Settings")]
    public float armor1Defense = 0.5f;
    public float armor2Defense = 1.0f;
    public float armor3Defense = 1.5f;

    public int armor1Cost = 50;
    public int armor2Cost = 100;
    public int armor3Cost = 150;

    [Header("Potion Settings")]
    public int healthPotionCost = 10;
    public int timerPotionCost = 30;
    public int invulnPotionCost = 50;

    public int maxPotionStack = 3;
    public int maxInvulnPotionStack = 1;

    private PlayerProfile profile;

    private void Start()
    {
        string profileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
        profile = SaveSystem.LoadProfile(profileName);

        if (profile == null)
        {
            Debug.LogError($"[ShopManager] Profile not found for {profileName}, creating default.");
            string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Ninja");
            profile = new PlayerProfile(profileName, selectedCharacter);
            profile.InitializeLevelStars();
            profile.InitializeMissionProgress();
            SaveSystem.SaveProfile(profile);
        }

        armor1Button.onClick.AddListener(BuyArmor1);
        armor2Button.onClick.AddListener(BuyArmor2);
        armor3Button.onClick.AddListener(BuyArmor3);

        buyHealthPotionButton.onClick.AddListener(BuyHealthPotion);
        buyTimerPotionButton.onClick.AddListener(BuyTimerPotion);
        buyInvulnPotionButton.onClick.AddListener(BuyInvulnPotion);

        RecalculateDefense();
        UpdateUI();
    }

    private void BuyArmor1() => TryBuyArmor(ref profile.armor1Unlocked, armor1Cost, armor1StatusText);
    private void BuyArmor2() => TryBuyArmor(ref profile.armor2Unlocked, armor2Cost, armor2StatusText);
    private void BuyArmor3() => TryBuyArmor(ref profile.armor3Unlocked, armor3Cost, armor3StatusText);

    private void TryBuyArmor(ref bool unlocked, int cost, Text statusText)
    {
        if (unlocked) return;

        if (GoldManager.Instance.goldAmount >= cost)
        {
            GoldManager.Instance.SpendGold(cost);
            unlocked = true;
            RecalculateDefense();
            GoldManager.Instance.SaveGold(); // Save gold first
            profile.gold = GoldManager.Instance.goldAmount; // Sync profile gold with GoldManager
            SaveSystem.SaveProfile(profile); // Save updated profile
            statusText.text = "Unlocked";
        }
        else
        {
            statusText.text = "Not enough gold";
        }

        UpdateUI();
    }

    private void RecalculateDefense()
    {
        float highestDefense = 0f;

        if (profile.armor3Unlocked) highestDefense = armor3Defense;
        else if (profile.armor2Unlocked) highestDefense = armor2Defense;
        else if (profile.armor1Unlocked) highestDefense = armor1Defense;

        profile.defense = highestDefense;
    }

    private void BuyHealthPotion()
    {
        if (profile.healthPotionCount >= maxPotionStack)
        {
            StartCoroutine(ShowTemporaryStatus(healthPotionStatusText, "Maxed", "+1 Health"));
            return;
        }

        if (GoldManager.Instance.goldAmount >= healthPotionCost)
        {
            GoldManager.Instance.SpendGold(healthPotionCost);
            profile.healthPotionCount++;
            GoldManager.Instance.SaveGold(); // Save gold first
            profile.gold = GoldManager.Instance.goldAmount; // Sync profile gold with GoldManager
            SaveSystem.SaveProfile(profile);
            StartCoroutine(ShowTemporaryStatus(healthPotionStatusText, "Purchased", "+1 Health"));
        }
        else
        {
            StartCoroutine(ShowTemporaryStatus(healthPotionStatusText, "Not enough gold", "+1 Health"));
        }

        UpdateUI();
    }

    private void BuyTimerPotion()
    {
        if (profile.timerPotionCount >= maxPotionStack)
        {
            StartCoroutine(ShowTemporaryStatus(timerPotionStatusText, "Maxed", "+15 seconds"));
            return;
        }

        if (GoldManager.Instance.goldAmount >= timerPotionCost)
        {
            GoldManager.Instance.SpendGold(timerPotionCost);
            profile.timerPotionCount++;
            GoldManager.Instance.SaveGold(); // Save gold first
            profile.gold = GoldManager.Instance.goldAmount; // Sync profile gold with GoldManager
            SaveSystem.SaveProfile(profile);
            StartCoroutine(ShowTemporaryStatus(timerPotionStatusText, "Purchased", "+15 seconds"));
        }
        else
        {
            StartCoroutine(ShowTemporaryStatus(timerPotionStatusText, "Not enough gold", "+15 seconds"));
        }

        UpdateUI();
    }

    private void BuyInvulnPotion()
    {
        if (profile.invulnerabilityPotionCount >= maxInvulnPotionStack)
        {
            StartCoroutine(ShowTemporaryStatus(invulnPotionStatusText, "Maxed", "Invulnerability"));
            return;
        }

        if (GoldManager.Instance.goldAmount >= invulnPotionCost)
        {
            GoldManager.Instance.SpendGold(invulnPotionCost);
            profile.invulnerabilityPotionCount++;
            GoldManager.Instance.SaveGold(); // Save gold first
            profile.gold = GoldManager.Instance.goldAmount; // Sync profile gold with GoldManager
            SaveSystem.SaveProfile(profile);
            StartCoroutine(ShowTemporaryStatus(invulnPotionStatusText, "Purchased", "Invulnerability"));
        }
        else
        {
            StartCoroutine(ShowTemporaryStatus(invulnPotionStatusText, "Not enough gold", "Invulnerability"));
        }

        UpdateUI();
    }

    private IEnumerator ShowTemporaryStatus(Text textElement, string temporaryText, string defaultText)
    {
        textElement.text = temporaryText;
        yield return new WaitForSeconds(1f);
        textElement.text = defaultText;
    }

    private void UpdateUI()
    {
        if (defenseValueText != null)
            defenseValueText.text = $"Defense: {profile.defense:F1}";

        UpdateArmorStatus(profile.armor1Unlocked, armor1Defense, armor1StatusText);
        UpdateArmorStatus(profile.armor2Unlocked, armor2Defense, armor2StatusText);
        UpdateArmorStatus(profile.armor3Unlocked, armor3Defense, armor3StatusText);

        armor1Button.interactable = !profile.armor1Unlocked;
        armor2Button.interactable = !profile.armor2Unlocked;
        armor3Button.interactable = !profile.armor3Unlocked;

        if (healthPotionNumberText != null)
            healthPotionNumberText.text = $"x{profile.healthPotionCount}/{maxPotionStack}";

        if (timerPotionNumberText != null)
            timerPotionNumberText.text = $"x{profile.timerPotionCount}/{maxPotionStack}";

        if (invulnPotionNumberText != null)
            invulnPotionNumberText.text = $"x{profile.invulnerabilityPotionCount}/{maxInvulnPotionStack}";
    }

    private void UpdateArmorStatus(bool unlocked, float defenseValue, Text statusText)
    {
        if (unlocked)
        {
            statusText.text = "Unlocked";
        }
        else if (statusText.text != "Not enough gold")
        {
            statusText.text = $"+{defenseValue} Defense";
        }
    }
}