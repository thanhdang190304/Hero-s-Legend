[System.Serializable]
public class PlayerProfile
{
    public string profileName;
    public string selectedCharacter;
    public int currentLevel;

    public float speed = 5f;
    public float jumpPower = 12f;
    public float attackCooldown = 1f;
    public int extraJumps = 0;
    public float wallJumpX = 0;
    public float wallJumpY = 0;

    public float upgradePoints = 0f;
    public int gold = 0;
    public bool doubleJumpUnlocked = false;
    public bool wallJumpUnlocked = false;
    public bool armor1Unlocked;
    public bool armor2Unlocked;
    public bool armor3Unlocked;
    public int healthPotionCount = 0;
    public int timerPotionCount = 0;
    public int invulnerabilityPotionCount = 0;

    public int maxPotionStack = 3;

    public float defense;

    public int[] levelStars;

    public bool tenStarRewardClaimed = false;
    public bool thirtyStarRewardClaimed = false;
    public bool fortyStarRewardClaimed = false;

    public int[] missionCurrentKills;
    public bool[] missionIsClaimed;
    public bool[] tookDamageInLevel;
    public bool[] levelsCompleted;

    public PlayerProfile(string profileName, string character)
    {
        this.profileName = profileName;
        this.selectedCharacter = character;
        this.currentLevel = 1;

        this.speed = 5f;
        this.jumpPower = 20f;
        this.attackCooldown = 1f;
        this.upgradePoints = 0f;
        this.gold = 0;
        this.extraJumps = 0;
        this.wallJumpX = 0;
        this.wallJumpY = 0;
        this.doubleJumpUnlocked = false;
        this.wallJumpUnlocked = false;

        this.defense = 0f;

        InitializeLevelStars();
        this.tenStarRewardClaimed = false;
        this.thirtyStarRewardClaimed = false;
        this.fortyStarRewardClaimed = false;

        InitializeMissionProgress();
    }

    public void InitializeLevelStars()
    {
        if (levelStars == null || levelStars.Length != 20)
        {
            levelStars = new int[20];
            for (int i = 0; i < levelStars.Length; i++)
            {
                levelStars[i] = 0;
            }
        }
    }

    public void InitializeMissionProgress()
    {
        // Adjusted for 6 missions (3 kill-based + 3 no-damage missions)
        if (missionCurrentKills == null || missionCurrentKills.Length != 6)
        {
            missionCurrentKills = new int[6];
            for (int i = 0; i < missionCurrentKills.Length; i++)
            {
                missionCurrentKills[i] = 0;
            }
        }
        if (missionIsClaimed == null || missionIsClaimed.Length != 6)
        {
            missionIsClaimed = new bool[6];
            for (int i = 0; i < missionIsClaimed.Length; i++)
            {
                missionIsClaimed[i] = false;
            }
        }
        if (tookDamageInLevel == null || tookDamageInLevel.Length != 20)
        {
            tookDamageInLevel = new bool[20];
            for (int i = 0; i < tookDamageInLevel.Length; i++)
            {
                tookDamageInLevel[i] = false;
            }
        }
        if (levelsCompleted == null || levelsCompleted.Length != 20)
        {
            levelsCompleted = new bool[20];
            for (int i = 0; i < levelsCompleted.Length; i++)
            {
                levelsCompleted[i] = false;
            }
        }
    }
}