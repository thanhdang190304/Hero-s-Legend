using UnityEngine;

[System.Serializable]
public class Mission
{
    public string description;
    public int killTarget;
    public int currentKills;
    public int goldReward;
    public bool isClaimed;
    public bool requiresNoDamage;
    public int levelIndex;

    public bool IsComplete
    {
        get
        {
            if (requiresNoDamage)
            {
                string profileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
                PlayerProfile profile = SaveSystem.LoadProfile(profileName);
                if (profile != null)
                {
                    profile.InitializeMissionProgress();
                    if (levelIndex >= 0 && levelIndex < profile.tookDamageInLevel.Length)
                    {
                        bool tookDamage = profile.tookDamageInLevel[levelIndex];
                        bool levelCompleted = profile.levelsCompleted[levelIndex];
                        bool result = levelCompleted && !tookDamage;
                        Debug.Log($"[Mission] Checking IsComplete for '{description}' (levelIndex={levelIndex}): tookDamage={tookDamage}, levelCompleted={levelCompleted}, Result={result}");
                        return result;
                    }
                    else
                    {
                        Debug.LogWarning($"[Mission] Invalid levelIndex {levelIndex} for '{description}'. Assuming incomplete.");
                        return false;
                    }
                }
                else
                {
                    Debug.LogWarning($"[Mission] Failed to load profile {profileName} for '{description}'. Assuming incomplete.");
                    return false;
                }
            }
            bool killComplete = currentKills >= killTarget;
            Debug.Log($"[Mission] Checking IsComplete for '{description}': currentKills={currentKills}, killTarget={killTarget}, Result={killComplete}");
            return killComplete;
        }
    }

    public void AddKill()
    {
        if (!isClaimed && !requiresNoDamage && currentKills < killTarget)
        {
            currentKills++;
            Debug.Log($"[Mission] {description} - {currentKills}/{killTarget}");
        }
    }
}