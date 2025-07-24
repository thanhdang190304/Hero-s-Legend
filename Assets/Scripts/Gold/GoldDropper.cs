using UnityEngine;

public class GoldDropper : MonoBehaviour
{
    [Tooltip("Assign the Gold prefab here")]
    public GameObject goldPrefab;

    [Tooltip("Chance to spawn gold (0 to 1). Example: 0.5 = 50% chance")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;

    public void TrySpawnGold()
{
    float randomValue = Random.Range(0f, 1f);

    Debug.Log("Trying to spawn gold. Random Value = " + randomValue);

    if (randomValue <= spawnChance)
    {
        Debug.Log("Gold spawned!");
        SpawnGold();
    }
}


    private void SpawnGold()
    {
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
    }
}
