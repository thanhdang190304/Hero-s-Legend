using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelStartManager : MonoBehaviour
{
    private const float maxWaitTime = 5f;
    private float waitInterval = 0.1f;

    void Start()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex - 3;
        Debug.Log($"[LevelStartManager] Scene loaded: {SceneManager.GetActiveScene().name}, BuildIndex: {SceneManager.GetActiveScene().buildIndex}, Calculated levelIndex: {levelIndex}");
        if (levelIndex >= 0 && levelIndex < 20)
        {
            StartCoroutine(WaitForPlayerAndStartTracking(levelIndex));
        }
        else
        {
            Debug.LogWarning($"[LevelStartManager] Invalid level index: {levelIndex}. Expected range: 0-19 (build indices 3-22).");
        }
    }

    private IEnumerator WaitForPlayerAndStartTracking(int levelIndex)
    {
        float elapsedTime = 0f;
        Health playerHealth = null;

        while (elapsedTime < maxWaitTime)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    Debug.Log($"[LevelStartManager] Player found with Health component after {elapsedTime} seconds.");
                    break;
                }
            }
            elapsedTime += waitInterval;
            yield return new WaitForSeconds(waitInterval);
        }

        if (playerHealth != null)
        {
            playerHealth.StartTrackingLevel(levelIndex);
            Debug.Log($"[LevelStartManager] Started tracking damage for level {levelIndex}");
        }
        else
        {
            Debug.LogError($"[LevelStartManager] Player Health component not found after waiting {maxWaitTime} seconds!");
        }
    }
}