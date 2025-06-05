using UnityEngine;
using UnityEngine.SceneManagement;

public class SetNumOfPlayers : MonoBehaviour
{
    [SerializeField] private GameObject _player2;
    [SerializeField] private GameObject _player3;
    [SerializeField] private GameObject _player4;
    private int _requestedNumOfPlayers;
    [SerializeField] private SettingsManager settings;
    [SerializeField] private SplitscreenManager splitscreenManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (settings == null)
        {
            Debug.LogWarning("why noi setting managerer");
        }
        _requestedNumOfPlayers = PlayerManager.Instance.NumOfPlayers;

        if (_requestedNumOfPlayers != 0 && SceneManager.GetActiveScene().buildIndex != 0)
        {
            settings.UpdatePlayerCount(_requestedNumOfPlayers);
            splitscreenManager.AdjustSplitscreenConfig();
        }
    }
}
