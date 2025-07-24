using UnityEngine;
using UnityEngine.UI;

public class CharacterIconDisplay : MonoBehaviour
{
    [SerializeField] private Image characterImage; // Assign this in the Inspector
    [SerializeField] private string characterIconFolder = "CharacterIcons"; // Icons should be in Resources/CharacterIcons/

    void Start()
    {
        // Get the active profile name from PlayerPrefs
        string activeProfileName = PlayerPrefs.GetString("ActiveProfile", null);
        Debug.Log("[CharacterIconDisplay] ActiveProfile from PlayerPrefs: " + activeProfileName);

        if (string.IsNullOrEmpty(activeProfileName))
        {
            Debug.LogError("[CharacterIconDisplay] No active profile found in PlayerPrefs.");
            return;
        }

        // Load the profile data
        PlayerProfile profile = SaveSystem.LoadProfile(activeProfileName);
        if (profile == null)
        {
            Debug.LogError("[CharacterIconDisplay] Failed to load profile: " + activeProfileName);
            return;
        }

        Debug.Log("[CharacterIconDisplay] Loaded profile: " + profile.profileName + ", Character: " + profile.selectedCharacter);

        // Load the sprite from Resources
        string spritePath = $"{characterIconFolder}/{profile.selectedCharacter}";
        Sprite charSprite = Resources.Load<Sprite>(spritePath);

        if (charSprite != null)
        {
            characterImage.sprite = charSprite;
            Debug.Log($"[CharacterIconDisplay] Successfully loaded character icon: Resources/{spritePath}");
        }
        else
        {
            Debug.LogError($"[CharacterIconDisplay] Character sprite not found at: Resources/{spritePath}");
        }
    }
}
