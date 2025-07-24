using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;
    public List<Mission> missions = new List<Mission>();
    private string lastKnownProfileName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupMissions();
            LoadMissionProgress();
            lastKnownProfileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (missions.Count == 0)
        {
            Debug.Log("[MissionManager] Mission list was empty in Start(), calling SetupMissions()");
            SetupMissions();
        }
    }

    private void Update()
    {
        string currentProfileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
        if (currentProfileName != lastKnownProfileName)
        {
            Debug.Log($"[MissionManager] ActiveProfile changed from {lastKnownProfileName} to {currentProfileName}. Resetting missions...");
            lastKnownProfileName = currentProfileName;
            SetupMissions();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("[MissionManager] Application quitting, saving mission progress...");
        SyncMissionsWithProfile();
    }

    public void ForceResetMissions()
    {
        Debug.Log("[MissionManager] Forcing mission reset...");
        SetupMissions();
    }

    private void SetupMissions()
    {
        Debug.Log("[MissionManager] Setting up missions...");
        missions.Clear();

        missions.Add(new Mission
        {
            description = "Kill 10 enemies for 15 gold",
            killTarget = 10,
            goldReward = 15,
            currentKills = 0,
            isClaimed = false,
            requiresNoDamage = false,
            levelIndex = -1
        });

        missions.Add(new Mission
        {
            description = "Kill 50 enemies for 100 gold",
            killTarget = 50,
            goldReward = 100,
            currentKills = 0,
            isClaimed = false,
            requiresNoDamage = false,
            levelIndex = -1
        });

        missions.Add(new Mission
        {
            description = "Kill 200 enemies for 100 gold",
            killTarget = 200,
            goldReward = 100,
            currentKills = 0,
            isClaimed = false,
            requiresNoDamage = false,
            levelIndex = -1
        });

        missions.Add(new Mission
        {
            description = "Defeat level 1 without losing any health for 10 gold",
            killTarget = 0,
            goldReward = 10,
            currentKills = 0,
            isClaimed = false,
            requiresNoDamage = true,
            levelIndex = 0
        });

        missions.Add(new Mission
        {
            description = "Defeat level 5 without losing any health for 30 gold",
            killTarget = 0,
            goldReward = 30,
            currentKills = 0,
            isClaimed = false,
            requiresNoDamage = true,
            levelIndex = 4
        });

        missions.Add(new Mission
        {
            description = "Defeat level 10 without losing any health for 50 gold",
            killTarget = 0,
            goldReward = 50,
            currentKills = 0,
            isClaimed = false,
            requiresNoDamage = true,
            levelIndex = 9
        });

        Debug.Log($"[MissionManager] Missions count after setup: {missions.Count}");
        LoadMissionProgress();
    }

    private void LoadMissionProgress()
    {
        string profileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
        Debug.Log($"[MissionManager] Loading mission progress for profile: {profileName}");
        PlayerProfile profile = SaveSystem.LoadProfile(profileName);
        if (profile != null)
        {
            profile.InitializeMissionProgress();
            for (int i = 0; i < missions.Count && i < profile.missionCurrentKills.Length; i++)
            {
                missions[i].currentKills = profile.missionCurrentKills[i];
                missions[i].isClaimed = profile.missionIsClaimed[i];
                Debug.Log($"[MissionManager] Loaded mission {i} ('{missions[i].description}'): Kills={missions[i].currentKills}/{missions[i].killTarget}, Claimed={missions[i].isClaimed}");
            }
        }
        else
        {
            Debug.LogWarning("[MissionManager] No profile found to load mission progress.");
            for (int i = 0; i < missions.Count; i++)
            {
                missions[i].currentKills = 0;
                missions[i].isClaimed = false;
                Debug.Log($"[MissionManager] Reset mission {i} ('{missions[i].description}'): Kills={missions[i].currentKills}, Claimed={missions[i].isClaimed}");
            }
        }
    }

    private void SyncMissionsWithProfile()
    {
        string profileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
        Debug.Log($"[MissionManager] Syncing missions with profile: {profileName}");
        PlayerProfile profile = SaveSystem.LoadProfile(profileName);
        if (profile != null)
        {
            profile.InitializeMissionProgress();
            for (int i = 0; i < missions.Count; i++)
            {
                if (i < profile.missionCurrentKills.Length)
                    profile.missionCurrentKills[i] = missions[i].currentKills;
                if (i < profile.missionIsClaimed.Length)
                    profile.missionIsClaimed[i] = missions[i].isClaimed;
                Debug.Log($"[MissionManager] Saving mission {i} ('{missions[i].description}'): Kills={missions[i].currentKills}, Claimed={missions[i].isClaimed}");
            }
            SaveSystem.SaveProfile(profile);
            Debug.Log("[MissionManager] Synced missions with profile and saved.");
        }
        else
        {
            Debug.LogWarning("[MissionManager] No profile found to sync missions.");
        }
    }

    public void EnemyKilled()
    {
        Debug.Log("[MissionManager] EnemyKilled() called.");
        foreach (var mission in missions)
        {
            if (!mission.isClaimed && mission.currentKills < mission.killTarget)
            {
                mission.AddKill();
                Debug.Log($"[MissionManager] Mission '{mission.description}' - Kills: {mission.currentKills}/{mission.killTarget}");
                if (mission.IsComplete)
                    Debug.Log($"[MissionManager] Mission '{mission.description}' is now COMPLETE!");
            }
        }
        SyncMissionsWithProfile();
    }

    public bool ClaimMission(Mission mission)
    {
        if (mission.IsComplete && !mission.isClaimed)
        {
            mission.isClaimed = true;
            string profileName = PlayerPrefs.GetString("ActiveProfile", "DefaultPlayer");
            var profile = SaveSystem.LoadProfile(profileName);
            if (profile != null)
            {
                profile.gold += mission.goldReward;
                SaveSystem.SaveProfile(profile);
                Debug.Log($"[MissionManager] Mission claimed! +{mission.goldReward} gold awarded to profile '{profileName}'. New gold: {profile.gold}");
                if (GoldManager.Instance != null)
                {
                    GoldManager.Instance.ReloadGoldFromProfile();
                }
                SyncMissionsWithProfile();
                return true;
            }
            else
            {
                Debug.LogWarning("[MissionManager] Could not find profile to add gold.");
            }
        }
        else
        {
            Debug.Log($"[MissionManager] Mission '{mission.description}' not claimed: IsComplete={mission.IsComplete}, IsClaimed={mission.isClaimed}");
        }
        return false;
    }
}