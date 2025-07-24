using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeLimit = 300f; // Set to 5 minutes
    private float currentTime;
    private bool timerActive = true;

    [Header("UI")]
    public Text timerText; // Assign in Inspector

    private Health playerHealth;

    void Start()
    {
<<<<<<< Updated upstream
        ResetTimer();
        InitializePlayerHealth();
        UpdateTimerUI();
    }

    private void InitializePlayerHealth()
    {
=======
        currentTime = timeLimit;
        Debug.Log($"[LevelTimer] Initialized with timeLimit: {timeLimit}, currentTime: {currentTime}");

>>>>>>> Stashed changes
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
<<<<<<< Updated upstream
            if (playerHealth == null)
            {
                Debug.LogError("[LevelTimer] Could not find Health script on Player!");
            }
            else
            {
                Debug.Log($"[LevelTimer] PlayerHealth initialized successfully. Current health: {playerHealth.currentHealth}");
            }
        }
        else
        {
            Debug.LogError("[LevelTimer] Could not find Player object with tag 'Player'!");
        }
    }

    private void ResetTimer()
    {
        currentTime = timeLimit;
        timerActive = true;
        Debug.Log($"[LevelTimer] Timer reset with timeLimit: {timeLimit}, currentTime: {currentTime}");
=======
        }

        if (playerHealth == null)
        {
            Debug.LogError("[LevelTimer] Could not find player or Health script!");
        }

        UpdateTimerUI();
>>>>>>> Stashed changes
    }

    void Update()
    {
        if (!timerActive)
            return;

        currentTime -= Time.deltaTime;
<<<<<<< Updated upstream
=======
       
>>>>>>> Stashed changes

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            TimerRanOut();
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
        else
        {
            Debug.LogError("[LevelTimer] TimerText not assigned!");
        }
    }

    private void TimerRanOut()
    {
        timerActive = false;
<<<<<<< Updated upstream
        Debug.Log("[LevelTimer] Timer ended — attempting to kill player!");

        // Reattempt to find player if playerHealth is null
        if (playerHealth == null)
        {
            InitializePlayerHealth();
        }

        if (playerHealth != null)
        {
            Debug.Log($"[LevelTimer] Player current health before damage: {playerHealth.currentHealth}");
            if (playerHealth.currentHealth > 0)
            {
                playerHealth.TakeDamage(100); // Instant death
                Debug.Log("[LevelTimer] Player dealt 100 damage.");
            }
            else
            {
                Debug.LogWarning("[LevelTimer] Player is already dead (currentHealth <= 0)!");
            }
        }
        else
        {
            Debug.LogError("[LevelTimer] PlayerHealth is still null after reinitialization!");
=======
        Debug.Log("[LevelTimer] Timer ended — killing player!");

        if (playerHealth != null && playerHealth.currentHealth > 0)
        {
            playerHealth.TakeDamage(100); // Instant death
>>>>>>> Stashed changes
        }
    }

    public void StopTimer()
    {
        timerActive = false;
        Debug.Log($"[LevelTimer] StopTimer called, currentTime: {currentTime}");
    }

    public void AddTime(float seconds)
    {
        timeLimit += seconds;
        currentTime += seconds;
        Debug.Log($"[LevelTimer] Added {seconds} seconds, new currentTime: {currentTime}");
    }

    // Getters
    public float GetCurrentTime() => currentTime;
    public float GetTimeLimit() => timeLimit;
<<<<<<< Updated upstream

    // Method to reset timer externally if needed
    public void ResetTimerExternally()
    {
        ResetTimer();
        InitializePlayerHealth();
        UpdateTimerUI();
    }
=======
>>>>>>> Stashed changes
}