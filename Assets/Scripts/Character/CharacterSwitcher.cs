using UnityEngine;

public class PlayerSwitching : MonoBehaviour
{
    public GameObject player1; // First character
    public GameObject player2; // Second character
    public GameObject player3; // Third character
    private GameObject activePlayer; // Currently active player

    private void Start()
    {
        // Set player1 as the default and disable others at the start
        activePlayer = player1;
        player1.SetActive(true);
        player2.SetActive(false);
        player3.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchToPlayer(player1);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            SwitchToPlayer(player2);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            SwitchToPlayer(player3);
        }
    }

    private void SwitchToPlayer(GameObject newPlayer)
    {
        if (activePlayer != newPlayer)
        {
            player1.SetActive(false);
            player2.SetActive(false);
            player3.SetActive(false);

            newPlayer.SetActive(true);
            activePlayer = newPlayer;
        }
    }
}
