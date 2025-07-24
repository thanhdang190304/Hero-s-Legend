using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    private const string ActiveProfileKey = "ActiveProfile";

    [Header("Hero Prefabs")]
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject dragonWarriorPrefab;
    [SerializeField] private GameObject ninjaPrefab;

    [Header("Spawn Location")]
    [SerializeField] private Transform spawnPoint;

    private GameObject currentPlayer;
    private string lastKnownProfileName;

    void Start()
    {
        Debug.Log("[Spawner] Scene started");
        lastKnownProfileName = PlayerPrefs.GetString(ActiveProfileKey, "(none)");
        SpawnCharacter();
    }

    void Update()
    {
        // Detect profile changes
        string activeProfileName = PlayerPrefs.GetString(ActiveProfileKey, "(none)");
        if (activeProfileName != lastKnownProfileName)
        {
            Debug.Log($"[Spawner] ActiveProfile changed from {lastKnownProfileName} to {activeProfileName}. Respawning character...");
            lastKnownProfileName = activeProfileName;
            SpawnCharacter();
        }
    }

    public void SpawnCharacter()
    {
        // Destroy existing player if any
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            Debug.Log("[Spawner] Destroyed existing player.");
        }

        // Read which profile is active
        string activeProfileName = PlayerPrefs.GetString(ActiveProfileKey, "(none)");
        Debug.Log($"[Spawner] ActiveProfileKey = \"{activeProfileName}\"");

        if (string.IsNullOrEmpty(activeProfileName))
        {
            Debug.LogError("[Spawner] ActiveProfileKey is empty – was HeroSelectionManager ever run?");
            return;
        }

        // Load the JSON for that profile
        PlayerProfile profile = SaveSystem.LoadProfile(activeProfileName);
        if (profile == null)
        {
            Debug.LogError($"[Spawner] profile JSON NOT found for \"{activeProfileName}\"");
            return;
        }
        Debug.Log($"[Spawner] Loaded profile. selectedCharacter = {profile.selectedCharacter}");

        // Choose the right prefab
        GameObject prefabToSpawn = profile.selectedCharacter switch
        {
            "Archer"        => archerPrefab,
            "DragonWarrior" => dragonWarriorPrefab,
            "Ninja"         => ninjaPrefab,
            _               => null
        };

        if (prefabToSpawn == null)
        {
            Debug.LogError($"[Spawner] No prefab assigned for character \"{profile.selectedCharacter}\"");
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("[Spawner] spawnPoint is NOT assigned in Inspector");
            return;
        }

        // Spawn!
        currentPlayer = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
        Debug.Log($"[Spawner] Instantiated {profile.selectedCharacter} prefab successfully");
    }
}