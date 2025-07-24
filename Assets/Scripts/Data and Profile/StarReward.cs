using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StarReward : MonoBehaviour
{
    [SerializeField] private Text rewardText; // Display level and star count
    [SerializeField] private Image[] starImages; // Array of star images (3)
    [SerializeField] private Text requirementsText; // Display star requirements
    [SerializeField] private Button continueButton; // Button to proceed

    void Start()
    {
        int starsEarned = PlayerPrefs.GetInt("StarsEarned", 0);
        int levelCompleted = PlayerPrefs.GetInt("LevelCompleted", 0);
        float timeLimit = PlayerPrefs.GetFloat("TimeLimit", 300f); // Default to 5 minutes if not set
        Debug.Log($"[StarReward] StarsEarned: {starsEarned}, LevelCompleted: {levelCompleted}, TimeLimit: {timeLimit}");

        // Update reward text
        if (rewardText != null)
        {
            rewardText.text = $"Level {levelCompleted} - {starsEarned} Stars!";
        }

        // Calculate dynamic requirements based on timeLimit
        float threeStarTime = timeLimit * 0.7f; // 70%
        float twoStarTime = timeLimit * 0.4f;   // 40%
        float oneStarTime = timeLimit * 0.1f;   // 10%

        // Convert to minutes and seconds for display
        string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }

        // Update requirements text
        if (requirementsText != null)
        {
            requirementsText.text = "Star Requirements:\n" +
                                   $"- 3 Stars: Finish with {FormatTime(threeStarTime)}+ remaining\n" +
                                   $"- 2 Stars: Finish with {FormatTime(twoStarTime)}-{FormatTime(threeStarTime - 0.01f)} remaining\n" +
                                   $"- 1 Star: Finish with {FormatTime(oneStarTime)}-{FormatTime(twoStarTime - 0.01f)} remaining\n" +
                                   $"- 0 Stars: Finish with less than {FormatTime(oneStarTime)} remaining";
        }

        // Enable stars based on count
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                starImages[i].gameObject.SetActive(i < starsEarned);
            }
            else
            {
                Debug.LogError($"[StarReward] StarImages[{i}] is not assigned!");
            }
        }

        // Set up continue button
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinue);
        }
    }

    void OnContinue()
    {
        SceneManager.LoadScene("_MainMenu_");
    }
}