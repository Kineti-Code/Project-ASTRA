using UnityEngine;

public class Bunker : MonoBehaviour
{
    private int playersInTrigger = 0;
    private GameObject[] players;
    [SerializeField] private float stormTriggerDistance = 5f;
    [SerializeField] private GameObject storm;
    [SerializeField] private GameObject EnterBunkerUI;
    public bool stormActivated = false;
    public bool stormSurvived = false;
    public bool allPlayersInBunker = false;

    private AudioManager audioManager;

    void Start()
    {

        if (storm != null)
        {
            storm.SetActive(false);
        }

        else
        {
            Debug.LogError("Storm not found");
        }

        if (EnterBunkerUI != null)
        {
            EnterBunkerUI.SetActive(false);
        }

        else
        {
            Debug.LogError("Enter Bunker UI not found");
        }

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Update()
    {
        // Check all players' positions regardless of trigger state
        if (!stormActivated && CheckPlayersInStormRange())
        {
            storm.SetActive(true);
            audioManager.PlaySFX(audioManager.impendingStorm);
            stormActivated = true;
        }

        if (!allPlayersInBunker && playersInTrigger == PlayerManager.Instance.NumOfPlayers)
        {
            EnterBunkerUI.SetActive(true);
        }

        else if (stormActivated && !stormSurvived)
        {
            EnterBunkerUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Return) && playersInTrigger > 0 && !stormSurvived)
        {
            if (playersInTrigger >= PlayerManager.Instance.NumOfPlayers)
            {
                Debug.Log("Entering bunker");
                allPlayersInBunker = true;

                // Set the sorting layer for all active players
                SetPlayersSortingLayer(0);
                TogglePlayerControls(false);
            }
            else
            {
                Debug.Log("More players needed");
            }
        }
    }

    public void TogglePlayerControls(bool controlsOn)
    {
        foreach (GameObject player in players)
        {
            // Get the Player script component
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                // Set controlsEnabled to false
                playerScript.controlsEnabled = controlsOn;
            }
        }
    }

    public void SetPlayersSortingLayer(int sortingLayerID)
    {
        // Find all active players in the scene
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // Find the "Visual" child object
            Transform visual = player.transform.Find("Visual");
            if (visual != null)
            {
                // Get the SpriteRenderer component and set the sorting layer
                SpriteRenderer renderer = visual.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = sortingLayerID;
                }
            }
        }
    }

    bool CheckPlayersInStormRange()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (Vector2.Distance(player.transform.position, transform.position) <= stormTriggerDistance)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger--;
        }
    }
}