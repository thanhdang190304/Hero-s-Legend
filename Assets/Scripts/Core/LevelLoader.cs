using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private int levelIndex;

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelIndex);
    }
}
