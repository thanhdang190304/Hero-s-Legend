using UnityEngine;

public class Gold : MonoBehaviour
{
    public int goldAmount = 1; // Amount of gold this pickup gives

    private void Update()
    {
        // Optional: play idle animation here if you want
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            // Get the GoldManager (or however you track gold) and add gold
            FindObjectOfType<GoldManager>().AddGold(1);


            // Destroy this gold object after pickup
            Destroy(gameObject);
        }
    }
}
