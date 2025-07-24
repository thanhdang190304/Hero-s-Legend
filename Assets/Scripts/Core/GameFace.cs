using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFace : MonoBehaviour
{
    [Header("Settings Menu")]
    [SerializeField] private GameObject soundTextObject; // Reference to the "Sound Text" GameObject
    [SerializeField] private GameObject musicTextObject; // Reference to the "Music Text" GameObject
    [SerializeField] private string soundVolumeKey = "soundVolume"; // PlayerPrefs key for sound volume
    [SerializeField] private string musicVolumeKey = "musicVolume"; // PlayerPrefs key for music volume

    private bool isSettingsOpen = false; // Tracks if settings menu is open

    private void Start()
    {
        Debug.Log("GameFace Start called");
        // Hide sound and music text at start
        if (soundTextObject != null)
        {
            soundTextObject.SetActive(false);
            Debug.Log("Sound Text Object disabled at start");
        }
        else
        {
            Debug.LogWarning("Sound Text Object is not assigned in the Inspector");
        }

        if (musicTextObject != null)
        {
            musicTextObject.SetActive(false);
            Debug.Log("Music Text Object disabled at start");
        }
        else
        {
            Debug.LogWarning("Music Text Object is not assigned in the Inspector");
        }

        // Ensure initial volume values are set
        if (!PlayerPrefs.HasKey(soundVolumeKey))
        {
            PlayerPrefs.SetFloat(soundVolumeKey, 1f); // Start at 100 (1 internally)
        }
        if (!PlayerPrefs.HasKey(musicVolumeKey))
        {
            PlayerPrefs.SetFloat(musicVolumeKey, 1f); // Start at 100 (1 internally)
        }
    }

    // Toggles the visibility of sound and music text
    public void ToggleSettingsMenu()
    {
        Debug.Log("ToggleSettingsMenu called on GameFace");
        if (HeroSelectionManager.CanProceed)
        {
            Debug.Log("HeroSelectionManager.CanProceed is true");
            isSettingsOpen = !isSettingsOpen;
            Debug.Log($"isSettingsOpen is now: {isSettingsOpen}");

            if (soundTextObject != null)
            {
                soundTextObject.SetActive(isSettingsOpen);
                Debug.Log($"Sound Text Object active state: {soundTextObject.activeSelf}");
            }
            else
            {
                Debug.LogWarning("Sound Text Object is not assigned in ToggleSettingsMenu");
            }

            if (musicTextObject != null)
            {
                musicTextObject.SetActive(isSettingsOpen);
                Debug.Log($"Music Text Object active state: {musicTextObject.activeSelf}");
            }
            else
            {
                Debug.LogWarning("Music Text Object is not assigned in ToggleSettingsMenu");
            }
        }
        else
        {
            Debug.LogWarning("[GameFace] ToggleSettingsMenu blocked due to active pop-up. HeroSelectionManager.CanProceed is false.");
        }
    }

    // Increases sound volume by 20 each click, wrapping around
    public void ToggleSoundVolume()
    {
        Debug.Log("ToggleSoundVolume called");
        if (HeroSelectionManager.CanProceed)
        {
            float currentVolume = PlayerPrefs.GetFloat(soundVolumeKey, 1f);
            float newVolume;

            // Increment by 0.2 (20%), wrap from 1 to 0.2
            if (currentVolume >= 1f)
            {
                newVolume = 0.2f; // Wrap from 100 to 20
            }
            else
            {
                newVolume = currentVolume + 0.2f; // Increase by 20
            }

            // Ensure precision (avoid floating-point drift)
            newVolume = Mathf.Round(newVolume * 10f) / 10f;

            if (SoundManager.instance != null)
            {
                SoundManager.instance.ChangeSoundVolume(newVolume - currentVolume);
                Debug.Log($"Sound volume changed from {currentVolume * 100} to {newVolume * 100}");
            }
            else
            {
                Debug.LogWarning("SoundManager.instance is null - volume change skipped");
            }

            PlayerPrefs.SetFloat(soundVolumeKey, newVolume); // Update PlayerPrefs
        }
        else
        {
            Debug.LogWarning("[GameFace] ToggleSoundVolume blocked due to active pop-up.");
        }
    }

    // Increases music volume by 20 each click, wrapping around
    public void ToggleMusicVolume()
    {
        Debug.Log("ToggleMusicVolume called");
        if (HeroSelectionManager.CanProceed)
        {
            float currentVolume = PlayerPrefs.GetFloat(musicVolumeKey, 1f);
            float newVolume;

            // Increment by 0.2 (20%), wrap from 1 to 0.2
            if (currentVolume >= 1f)
            {
                newVolume = 0.2f; // Wrap from 100 to 20
            }
            else
            {
                newVolume = currentVolume + 0.2f; // Increase by 20
            }

            // Ensure precision (avoid floating-point drift)
            newVolume = Mathf.Round(newVolume * 10f) / 10f;

            if (SoundManager.instance != null)
            {
                SoundManager.instance.ChangeMusicVolume(newVolume - currentVolume);
                Debug.Log($"Music volume changed from {currentVolume * 100} to {newVolume * 100}");
            }
            else
            {
                Debug.LogWarning("SoundManager.instance is null - volume change skipped");
            }

            PlayerPrefs.SetFloat(musicVolumeKey, newVolume); // Update PlayerPrefs
        }
        else
        {
            Debug.LogWarning("[GameFace] ToggleMusicVolume blocked due to active pop-up.");
        }
    }

    #region Scene Loading Methods
    public void LoadHeroSelect()
    {
        if (HeroSelectionManager.CanProceed)
        {
            SceneManager.LoadScene("_HeroSelect_"); // Make sure this matches the exact scene name
        }
        else
        {
            Debug.LogWarning("[GameFace] LoadHeroSelect blocked due to active pop-up.");
        }
    }

    public void LoadMainMenu()
    {
        if (HeroSelectionManager.CanProceed)
        {
            SceneManager.LoadScene("_MainMenu_"); // Make sure this matches the exact scene name
        }
        else
        {
            Debug.LogWarning("[GameFace] LoadMainMenu blocked due to active pop-up.");
        }
    }

    public void LoadMission()
    {
        if (HeroSelectionManager.CanProceed)
        {
            SceneManager.LoadScene("_Mission_"); // Make sure this matches the exact scene name
        }
        else
        {
            Debug.LogWarning("[GameFace] LoadMission blocked due to active pop-up.");
        }
    }

    public void LoadProfileSelection()
    {
        if (HeroSelectionManager.CanProceed)
        {
            SceneManager.LoadScene("_ProfileSelection_"); // Make sure this matches the exact scene name
        }
        else
        {
            Debug.LogWarning("[GameFace] LoadProfileSelection blocked due to active pop-up.");
        }
    }

    public void LoadStarsInformation()
    {
        if (HeroSelectionManager.CanProceed)
        {
            SceneManager.LoadScene("_StarsInformation_"); // Make sure this matches the exact scene name
        }
        else
        {
            Debug.LogWarning("[GameFace] LoadStarsInformation blocked due to active pop-up.");
        }
    }
    public void LoadGameInstruction()
    {
       
            SceneManager.LoadScene("_GameInstruction_"); // Make sure this matches the exact scene name   
    }
<<<<<<< Updated upstream
    public void LoadGameFace()
    {
       
            SceneManager.LoadScene("_GameFace_"); // Make sure this matches the exact scene name   
    }

     public void Tutorial()
    {
       
            SceneManager.LoadScene("_Tutorial_"); // Make sure this matches the exact scene name   
    }
=======
>>>>>>> Stashed changes

    public void Quit()
    {
        Application.Quit(); // Quits the game (only works in build)

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exits play mode (only in Editor)
#endif
    }
    #endregion
}