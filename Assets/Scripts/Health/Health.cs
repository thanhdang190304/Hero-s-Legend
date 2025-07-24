using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("Defense (only applies if tagged 'Player')")]
    [SerializeField] private float defense = 0f;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    [Header("Heal Sound")]
    [SerializeField] private AudioClip healSound;

    [Header("Respawn Effect")]
    [SerializeField] private GameObject respawnEffectPrefab;

    [Header("Enemy Damage Effect")]
    [SerializeField] private GameObject enemyDamageEffectPrefab;

    private bool hasTakenDamageInLevel = false;
    private bool isTrackingLevel = false;
    private int currentLevelIndex = -1;

    private void Awake()
    {
        try
        {
            currentHealth = startingHealth;
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
            Debug.Log($"[Health] Awake: {gameObject.name}, startingHealth={startingHealth}, tag={tag}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] Awake error for {gameObject.name}: {e.Message}");
        }
    }

    private void Start()
    {
        try
        {
            if (CompareTag("Player"))
            {
                string profileName = PlayerPrefs.GetString("ActiveProfile", null);
                if (!string.IsNullOrEmpty(profileName))
                {
                    PlayerProfile profile = SaveSystem.LoadProfile(profileName);
                    if (profile != null)
                    {
                        if (profile.defense <= 0f && defense > 0f)
                        {
                            profile.defense = defense;
                            SaveSystem.SaveProfile(profile);
                            Debug.Log($"[Health] Saved Inspector defense ({defense}) to profile for {profileName}.");
                        }
                        else
                        {
                            defense = profile.defense;
                            Debug.Log($"[Health] Loaded defense from profile: {defense} for {profileName}.");
                        }

                        bool isNewProfile = true;
                        for (int i = 0; i < profile.levelsCompleted.Length; i++)
                        {
                            if (profile.levelsCompleted[i])
                            {
                                isNewProfile = false;
                                break;
                            }
                        }

                        if (isNewProfile)
                        {
                            for (int i = 0; i < profile.tookDamageInLevel.Length; i++)
                            {
                                profile.tookDamageInLevel[i] = false;
                                profile.levelsCompleted[i] = false;
                            }
                            SaveSystem.SaveProfile(profile);
                            Debug.Log($"[Health] Detected new profile for {profileName}. Reset tookDamageInLevel and levelsCompleted arrays.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[Health] Could not load PlayerProfile for {profileName}.");
                    }
                }
                else
                {
                    Debug.LogWarning("[Health] No active profile set in PlayerPrefs.");
                }
            }
            Debug.Log($"[Health] Start: {gameObject.name}, currentHealth={currentHealth}, isTrackingLevel={isTrackingLevel}, hasTakenDamageInLevel={hasTakenDamageInLevel}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] Start error for {gameObject.name}: {e.Message}");
        }
    }

    public void StartTrackingLevel(int levelIndex)
    {
        try
        {
            isTrackingLevel = true;
            currentLevelIndex = levelIndex;
            hasTakenDamageInLevel = false;
            Debug.Log($"[Health] Started tracking level {levelIndex} for {gameObject.name}, isTrackingLevel={isTrackingLevel}, hasTakenDamageInLevel={hasTakenDamageInLevel}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] StartTrackingLevel error for {gameObject.name}: {e.Message}");
        }
    }

    public void EndTrackingLevel(int levelIndex)
    {
        try
        {
            if (currentLevelIndex == levelIndex)
            {
                string profileName = PlayerPrefs.GetString("ActiveProfile", null);
                if (!string.IsNullOrEmpty(profileName))
                {
                    PlayerProfile profile = SaveSystem.LoadProfile(profileName);
                    if (profile != null)
                    {
                        profile.InitializeMissionProgress();
                        if (levelIndex >= 0 && levelIndex < profile.tookDamageInLevel.Length)
                        {
                            profile.tookDamageInLevel[levelIndex] = hasTakenDamageInLevel;
                            profile.levelsCompleted[levelIndex] = true;
                            SaveSystem.SaveProfile(profile);
                            Debug.Log($"[Health] Saved state for level {levelIndex}: tookDamageInLevel[{levelIndex}]={hasTakenDamageInLevel}, levelsCompleted[{levelIndex}]=true");
                        }
                        else
                        {
                            Debug.LogError($"[Health] Level index {levelIndex} out of bounds for tookDamageInLevel array.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"[Health] Failed to load profile {profileName} to save damage state.");
                    }
                }
                else
                {
                    Debug.LogError("[Health] No active profile found to save damage state.");
                }

                isTrackingLevel = false;
                Debug.Log($"[Health] Ended tracking level {levelIndex}, damage taken: {hasTakenDamageInLevel}, currentLevelIndex={currentLevelIndex}");
                currentLevelIndex = -1;
            }
            else
            {
                Debug.LogWarning($"[Health] EndTrackingLevel called for level {levelIndex}, but currentLevelIndex is {currentLevelIndex}. No action taken.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] EndTrackingLevel error for {gameObject.name}: {e.Message}");
        }
    }

    public bool DidTakeDamageInLevel(int levelIndex)
    {
        try
        {
            bool result = currentLevelIndex == levelIndex && hasTakenDamageInLevel;
            Debug.Log($"[Health] DidTakeDamageInLevel({levelIndex}) called: currentLevelIndex={currentLevelIndex}, hasTakenDamageInLevel={hasTakenDamageInLevel}, Result={result}");
            return result;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] DidTakeDamageInLevel error for {gameObject.name}: {e.Message}");
            return false;
        }
    }

    public void TakeDamage(float _damage)
    {
        try
        {
            Debug.Log($"[Health] TakeDamage called for {gameObject.name} with damage={_damage}, invulnerable={invulnerable}, defense={defense}, currentHealth={currentHealth}");

            if (invulnerable)
            {
                Debug.Log("[Health] Took no damage because invulnerable.");
                return;
            }

            float finalDamage = _damage;

            if (CompareTag("Player"))
            {
<<<<<<< Updated upstream
                // New damage calculation: damage / (defense + 1), capped at 1.5
                float effectiveDefense = Mathf.Min(defense, 1.5f);
                finalDamage = _damage / (effectiveDefense + 1);
                Debug.Log($"[Health] Player damage calculated: {finalDamage:F1} (original: {_damage}, effective defense: {effectiveDefense})");
=======
                finalDamage = Mathf.Max(0, _damage - defense);
                Debug.Log($"[Health] Player damage calculated: {finalDamage} (original: {_damage}, defense: {defense})");
>>>>>>> Stashed changes
            }
            else
            {
                Debug.Log($"[Health] Enemy damage taken: {finalDamage}");
            }

            currentHealth = Mathf.Clamp(currentHealth - finalDamage, 0, startingHealth);
            Debug.Log($"[Health] Current health is now: {currentHealth} after taking {finalDamage} damage");

            if (currentHealth > 0)
            {
                anim.SetTrigger("hurt");
                StartCoroutine(Invunerability());
                SoundManager.instance.PlaySound(hurtSound);
                if (isTrackingLevel && !hasTakenDamageInLevel && finalDamage > 0)
                {
                    hasTakenDamageInLevel = true;
                    Debug.Log($"[Health] Player took damage in level {currentLevelIndex}. hasTakenDamageInLevel set to {hasTakenDamageInLevel}");
                }
                // Play enemy damage effect if this is an enemy
                if (CompareTag("Enemy") && enemyDamageEffectPrefab != null)
                {
                    GameObject effect = Instantiate(enemyDamageEffectPrefab, transform.position, Quaternion.identity);
                    Destroy(effect, 2f);
                    Debug.Log($"[Health] Enemy damage effect played for {gameObject.name} at position {transform.position}.");
                }
                else if (CompareTag("Enemy") && enemyDamageEffectPrefab == null)
                {
                    Debug.LogWarning($"[Health] Enemy damage effect prefab not assigned for {gameObject.name}!");
                }
            }
            else if (!dead)
            {
                Debug.Log($"[Health] {gameObject.name} died. Tag: {tag}");

                if (CompareTag("Enemy"))
                {
                    Debug.Log("[Health] Enemy killed — notifying MissionManager.");
                    MissionManager.instance?.EnemyKilled();
                    GetComponent<GoldDropper>()?.TrySpawnGold();
                }

                foreach (Behaviour component in components)
                    component.enabled = false;

                anim.SetBool("grounded", true);
                anim.SetTrigger("die");
                dead = true;

                SoundManager.instance.PlaySound(deathSound);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] TakeDamage error for {gameObject.name}: {e.Message}");
        }
    }

    public void AddHealth(float _value)
    {
        try
        {
            currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);

            if (CompareTag("Player") && _value > 0)
            {
                if (healSound != null)
                {
                    SoundManager.instance.PlaySound(healSound);
                    Debug.Log("[Health] Heal sound played for Player.");
                }
                else
                {
                    Debug.LogWarning("[Health] Heal sound not assigned!");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] AddHealth error for {gameObject.name}: {e.Message}");
        }
    }

    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    public void SetInvulnerable(bool value)
    {
        try
        {
            invulnerable = value;
            if (value)
            {
                spriteRend.color = Color.yellow;
                Debug.Log("[Health] Manual invulnerability activated.");
            }
            else
            {
                spriteRend.color = Color.white;
                Debug.Log("[Health] Manual invulnerability ended.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] SetInvulnerable error for {gameObject.name}: {e.Message}");
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        try
        {
            currentHealth = startingHealth;
            dead = false;
            anim.ResetTrigger("die");
            anim.SetBool("grounded", true);
            anim.SetBool("run", false);
            anim.Play("Idle", 0, 0f);
            anim.Update(0f);

            foreach (Behaviour component in components)
                component.enabled = true;

            StartCoroutine(Invunerability());

            if (respawnEffectPrefab != null)
            {
                GameObject effect = Instantiate(respawnEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
                Debug.Log("[Health] Respawn effect played.");
            }
            else
            {
                Debug.LogWarning("[Health] Respawn effect prefab not assigned!");
            }

            Debug.Log("[Health] Player respawned, health restored, and animation reset to Idle.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Health] Respawn error for {gameObject.name}: {e.Message}");
        }
    }
}