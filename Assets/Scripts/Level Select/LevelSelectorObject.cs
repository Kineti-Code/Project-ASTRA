using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorObject : MonoBehaviour
{
    [SerializeField]
    private bool playerInTrigger = false;
    [SerializeField] private int numPlayersInTrigger = 0;
    [SerializeField] private GameObject _openLevelSelectCanvas;

    private void Start()
    {
        if (_openLevelSelectCanvas != null)
            _openLevelSelectCanvas.SetActive(false);
    }

    private void Update()
    {
        if (playerInTrigger && numPlayersInTrigger == PlayerManager.Instance.NumOfPlayers)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("going to level select");
                SceneManager.LoadScene(1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            numPlayersInTrigger++;

            if (numPlayersInTrigger == PlayerManager.Instance.NumOfPlayers && _openLevelSelectCanvas != null)
            {
                _openLevelSelectCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
            numPlayersInTrigger--;

            if (_openLevelSelectCanvas != null)
            {
                _openLevelSelectCanvas.SetActive(false);
            }
        }
    }
}