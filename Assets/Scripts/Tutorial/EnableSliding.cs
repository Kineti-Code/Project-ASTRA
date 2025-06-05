using UnityEngine;

public class EnableSliding : MonoBehaviour
{

    private Player[] players;
    public int numPlayersTriggered = 0;

    void Start()
    {
        players = FindObjectsByType<Player>(FindObjectsSortMode.None);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            numPlayersTriggered++;

            if (numPlayersTriggered == PlayerManager.Instance.NumOfPlayers)
            {
                foreach (Player player in players)
                {
                    player.inTutorialMode = false;
                    player.GetComponentInChildren<KeybindController>().EnableSlide();
                }
            }
        }
    }
}
