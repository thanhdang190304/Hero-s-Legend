using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMainMenuTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered by: {other.gameObject.name}, Tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected, attempting to load _MainMenu_");
            SceneManager.LoadScene("_MainMenu_");
        }
    }
}