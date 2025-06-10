using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    public bool speedrunning = false;
    [SerializeField] private int _artifactsRequired = 3;

    [Header("Tablet Components")]
    [SerializeField] private GameObject _tabletMain;
    [SerializeField] private GameObject _levelCompleteUI;
    [SerializeField] private PopupMessenger popupMessenger;
    [SerializeField] private GameObject _enterToComplete;

    [Header("Level Data")]
    [SerializeField] private int levelNum;

    [Header("Extra parameters")]
    [SerializeField] private bool lastLevel = false;
    private string levelName => $"Level{levelNum}";

    private int numPlayersFinished = 0;
    private int currentScene;

    private void Start()
    {
        _enterToComplete.SetActive(false);
        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        Debug.Log(levelName);
    }

    private int GetArtifactsCollected()
    {
        int artifactsCollected = 0;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Player playerscript = player.GetComponent<Player>();
            artifactsCollected += playerscript.artifactsCollected;
        }

        return artifactsCollected;
    }

    private void Update()
    {
        if (PlayerManager.Instance != null)
        {
            if (numPlayersFinished == PlayerManager.Instance.NumOfPlayers)
            {

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    int artifactsCollected = GetArtifactsCollected();

                    if (artifactsCollected >= _artifactsRequired) // level completed
                    {
                        if (lastLevel)
                        {
                            PlayerPrefs.SetInt("Game Completed", 1);
                            SceneManager.LoadScene(2);
                        }

                        else if (speedrunning && currentScene + 1 <= 8)
                        {
                            SceneManager.LoadScene(currentScene + 1);
                        }

                        else
                        {
                            _tabletMain.SetActive(true);
                            _levelCompleteUI.SetActive(true);
                            PlayerPrefs.SetInt(levelName, 1);
                        }
                    }
                    else
                    {
                        string pOrS = _artifactsRequired - artifactsCollected == 1 ? "" : "s";
                        popupMessenger.OpenTabletMessage("More artifacts needed", new string[] { $"Looks like you don't have enough artifacts yet. You need {_artifactsRequired - artifactsCollected} more artifact{pOrS} to finish." });
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            numPlayersFinished--;

            if (_enterToComplete != null)
            {
                _enterToComplete.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            numPlayersFinished++;

            if (numPlayersFinished == PlayerManager.Instance.NumOfPlayers && _enterToComplete != null)
            {
                _enterToComplete.SetActive(true);
            }
        }
    }
}
