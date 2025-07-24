using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private Button ninjaSkillButton;
    [SerializeField] private Button archerSkillButton;
    [SerializeField] private Button dragonWarriorSkillButton;
    [SerializeField] private GameObject ninjaClonePrefab;
    [SerializeField] private Image ninjaCooldownImage;
    [SerializeField] private Image archerCooldownImage;
    [SerializeField] private Image dragonWarriorCooldownImage;

    private PlayerProfile activeProfile;
    private GameObject ninjaInstance;
    private bool isNinjaSkillOnCooldown = false;
    private float ninjaSkillCooldown = 10f;
    private bool isArcherSkillOnCooldown = false;
    private float archerSkillCooldown = 10f;
    private bool isDragonWarriorSkillOnCooldown = false;
    private float dragonWarriorSkillCooldown = 10f;

    private bool isPiercingActive = false;
    private float piercingDuration = 5f;
    private bool isBurningEffectActive = false; // Flag for burning effect
    private float burningEffectDuration = 5f; // Duration of burning effect window

    public bool IsPiercingActive => isPiercingActive;
    public bool IsBurningEffectActive => isBurningEffectActive; // Public getter for Projectile.cs

    private void Awake()
    {
        if (ninjaSkillButton != null) ninjaSkillButton.gameObject.SetActive(false);
        if (archerSkillButton != null) archerSkillButton.gameObject.SetActive(false);
        if (dragonWarriorSkillButton != null) dragonWarriorSkillButton.gameObject.SetActive(false);

        if (ninjaCooldownImage != null)
        {
            ninjaCooldownImage.fillAmount = 0f;
            ninjaCooldownImage.gameObject.SetActive(false);
        }

        if (archerCooldownImage != null)
        {
            archerCooldownImage.fillAmount = 0f;
            archerCooldownImage.gameObject.SetActive(false);
        }

        if (dragonWarriorCooldownImage != null)
        {
            dragonWarriorCooldownImage.fillAmount = 0f;
            dragonWarriorCooldownImage.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        string activeProfileName = PlayerPrefs.GetString("ActiveProfile", null);
        if (!string.IsNullOrEmpty(activeProfileName))
        {
            activeProfile = SaveSystem.LoadProfile(activeProfileName);
            if (activeProfile != null)
            {
                Debug.Log($"[SkillManager] Loaded profile for: {activeProfile.selectedCharacter}");
                SetupSkillButtons();
            }
        }

        UpdateNinjaInstance();
    }

<<<<<<< Updated upstream
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && activeProfile != null)
        {
            switch (activeProfile.selectedCharacter)
            {
                case "Ninja":
                    if (!isNinjaSkillOnCooldown)
                        SpawnNinjaClones();
                    break;
                case "Archer":
                    if (!isArcherSkillOnCooldown)
                        UseArcherSkill();
                    break;
                case "DragonWarrior":
                    if (!isDragonWarriorSkillOnCooldown)
                        UseDragonWarriorSkill();
                    break;
            }
        }
    }

=======
>>>>>>> Stashed changes
    private void SetupSkillButtons()
    {
        if (activeProfile == null)
            return;

        if (ninjaSkillButton != null) ninjaSkillButton.gameObject.SetActive(false);
        if (archerSkillButton != null) archerSkillButton.gameObject.SetActive(false);
        if (dragonWarriorSkillButton != null) dragonWarriorSkillButton.gameObject.SetActive(false);

        switch (activeProfile.selectedCharacter)
        {
            case "Ninja":
                if (ninjaSkillButton != null)
                {
                    ninjaSkillButton.gameObject.SetActive(true);
                    ninjaSkillButton.onClick.RemoveAllListeners();
                    ninjaSkillButton.onClick.AddListener(SpawnNinjaClones);
                    Debug.Log("[SkillManager] Showing Ninja skill button");
                }
                break;
            case "Archer":
                if (archerSkillButton != null)
                {
                    archerSkillButton.gameObject.SetActive(true);
                    archerSkillButton.onClick.RemoveAllListeners();
                    archerSkillButton.onClick.AddListener(UseArcherSkill);
                    Debug.Log("[SkillManager] Showing Archer skill button");
                }
                break;
            case "DragonWarrior":
                if (dragonWarriorSkillButton != null)
                {
                    dragonWarriorSkillButton.gameObject.SetActive(true);
                    dragonWarriorSkillButton.onClick.RemoveAllListeners();
                    dragonWarriorSkillButton.onClick.AddListener(UseDragonWarriorSkill);
                    Debug.Log("[SkillManager] Showing DragonWarrior skill button");
                }
                break;
        }
    }

    public void UpdateNinjaInstance()
    {
        ninjaInstance = GameObject.FindGameObjectWithTag("Player");
        if (ninjaInstance == null)
        {
            Debug.LogError("[SkillManager] Could not find Player-tagged GameObject after respawn");
        }
        else
        {
            Debug.Log("[SkillManager] Updated ninjaInstance to: " + ninjaInstance.name);
            SetupSkillButtons();
        }
    }

    private void SpawnNinjaClones()
    {
        if (isNinjaSkillOnCooldown)
        {
            Debug.Log("[SkillManager] Ninja skill is on cooldown");
            return;
        }

        if (ninjaInstance == null || ninjaClonePrefab == null)
        {
            Debug.LogError("[SkillManager] Cannot spawn clones: Ninja instance or prefab missing");
            UpdateNinjaInstance();
            if (ninjaInstance == null || ninjaClonePrefab == null)
            {
                Debug.LogError("[SkillManager] Recovery failed: Still missing Ninja instance or prefab");
                return;
            }
        }

        NinjaAttack ninjaAttack = ninjaInstance.GetComponent<NinjaAttack>();
        int damage = ninjaAttack != null ? ninjaAttack.AttackDamage : 1;

        Vector2 ninjaPos = ninjaInstance.transform.position;
        GameObject clone1 = Instantiate(ninjaClonePrefab, ninjaPos + Vector2.right * 1f, Quaternion.identity);
        GameObject clone2 = Instantiate(ninjaClonePrefab, ninjaPos + Vector2.left * 1f, Quaternion.identity);

        NinjaCloneAI cloneAI1 = clone1.GetComponent<NinjaCloneAI>();
        NinjaCloneAI cloneAI2 = clone2.GetComponent<NinjaCloneAI>();
        if (cloneAI1 != null) cloneAI1.attackDamage = damage;
        if (cloneAI2 != null) cloneAI2.attackDamage = damage;

        Debug.Log("[SkillManager] Spawned 2 Ninja clones");

        StartCoroutine(NinjaSkillCooldown());
    }

    private void UseArcherSkill()
    {
        if (isArcherSkillOnCooldown)
        {
            Debug.Log("[SkillManager] Archer skill is on cooldown");
            return;
        }

        Debug.Log("[SkillManager] Archer skill activated - Piercing mode enabled for 5 seconds");
        StartCoroutine(EnablePiercingMode());
        StartCoroutine(ArcherSkillCooldown());
    }

    private void UseDragonWarriorSkill()
    {
        if (isDragonWarriorSkillOnCooldown)
        {
            Debug.Log("[SkillManager] DragonWarrior skill is on cooldown");
            return;
        }

        Debug.Log("[SkillManager] DragonWarrior skill activated - Burning effect enabled for 5 seconds");
        StartCoroutine(EnableBurningEffect());
        StartCoroutine(DragonWarriorSkillCooldown());
    }

    private IEnumerator EnablePiercingMode()
    {
        isPiercingActive = true;
        Debug.Log("[SkillManager] Piercing mode active");

        yield return new WaitForSeconds(piercingDuration);

        isPiercingActive = false;
        Debug.Log("[SkillManager] Piercing mode ended");
    }

    private IEnumerator EnableBurningEffect()
    {
        isBurningEffectActive = true;
        Debug.Log("[SkillManager] Burning effect active");

        yield return new WaitForSeconds(burningEffectDuration);

        isBurningEffectActive = false;
        Debug.Log("[SkillManager] Burning effect ended");
    }

    private IEnumerator NinjaSkillCooldown()
    {
        isNinjaSkillOnCooldown = true;
        ninjaSkillButton.interactable = false;

        if (ninjaCooldownImage != null)
        {
            ninjaCooldownImage.gameObject.SetActive(true);
            ninjaCooldownImage.fillAmount = 1f;
        }

        Debug.Log("[SkillManager] Ninja skill on cooldown for 10 seconds");

        float elapsedTime = 0f;
        while (elapsedTime < ninjaSkillCooldown)
        {
            elapsedTime += Time.deltaTime;
            if (ninjaCooldownImage != null)
            {
                ninjaCooldownImage.fillAmount = 1f - (elapsedTime / ninjaSkillCooldown);
            }
            yield return null;
        }

        isNinjaSkillOnCooldown = false;
        ninjaSkillButton.interactable = true;

        if (ninjaCooldownImage != null)
        {
            ninjaCooldownImage.fillAmount = 0f;
            ninjaCooldownImage.gameObject.SetActive(false);
        }

        Debug.Log("[SkillManager] Ninja skill cooldown finished");
    }

    private IEnumerator ArcherSkillCooldown()
    {
        isArcherSkillOnCooldown = true;
        archerSkillButton.interactable = false;

        if (archerCooldownImage != null)
        {
            archerCooldownImage.gameObject.SetActive(true);
            archerCooldownImage.fillAmount = 1f;
        }

        Debug.Log("[SkillManager] Archer skill on cooldown for 10 seconds");

        float elapsedTime = 0f;
        while (elapsedTime < archerSkillCooldown)
        {
            elapsedTime += Time.deltaTime;
            if (archerCooldownImage != null)
            {
                archerCooldownImage.fillAmount = 1f - (elapsedTime / archerSkillCooldown);
            }
            yield return null;
        }

        isArcherSkillOnCooldown = false;
        archerSkillButton.interactable = true;

        if (archerCooldownImage != null)
        {
            archerCooldownImage.fillAmount = 0f;
            archerCooldownImage.gameObject.SetActive(false);
        }

        Debug.Log("[SkillManager] Archer skill cooldown finished");
    }

    private IEnumerator DragonWarriorSkillCooldown()
    {
        isDragonWarriorSkillOnCooldown = true;
        dragonWarriorSkillButton.interactable = false;

        if (dragonWarriorCooldownImage != null)
        {
            dragonWarriorCooldownImage.gameObject.SetActive(true);
            dragonWarriorCooldownImage.fillAmount = 1f;
        }

        Debug.Log("[SkillManager] DragonWarrior skill on cooldown for 10 seconds");

        float elapsedTime = 0f;
        while (elapsedTime < dragonWarriorSkillCooldown)
        {
            elapsedTime += Time.deltaTime;
            if (dragonWarriorCooldownImage != null)
            {
                dragonWarriorCooldownImage.fillAmount = 1f - (elapsedTime / dragonWarriorSkillCooldown);
            }
            yield return null;
        }

        isDragonWarriorSkillOnCooldown = false;
        dragonWarriorSkillButton.interactable = true;

        if (dragonWarriorCooldownImage != null)
        {
            dragonWarriorCooldownImage.fillAmount = 0f;
            dragonWarriorCooldownImage.gameObject.SetActive(false);
        }

        Debug.Log("[SkillManager] DragonWarrior skill cooldown finished");
    }
}