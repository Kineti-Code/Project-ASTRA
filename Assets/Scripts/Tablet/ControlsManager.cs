using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    [SerializeField] private GameObject _tabletMain;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _controlsUI;
    [SerializeField] private GameObject[] _playerControls;
    [SerializeField] private GameObject _backToPauseMenu;
    [SerializeField] private PauseManager _pauseManager;

    void OnEnable()
    {

        if (!_tabletMain.activeSelf)
        {
            _tabletMain.SetActive(true);
        }

        _controlsUI.SetActive(true);
        Time.timeScale = 0;

        if (_backToPauseMenu != null)
        {
            _backToPauseMenu.SetActive(false);
        }

        foreach (var control in _playerControls)
        {
            control.SetActive(false);
        }

        ShowPauseButton();
        ShowControls();
    }

    private void ShowControls()
    {
        int numPlayers = PlayerManager.Instance.NumOfPlayers;

        for (int i = 0; i < numPlayers; i++)
        {
            _playerControls[i].SetActive(true);
        }
    }

    public void ShowPauseButton()
    {
        if (_pauseManager != null && _pauseManager.fromPauseMenu)
        {
            if (_backToPauseMenu != null)
            {
                _backToPauseMenu.SetActive(true);
            }
            _pauseManager.fromPauseMenu = false;
        }
    }

    public void ExitControls()
    {
        Time.timeScale = 1;
        _controlsUI.SetActive(false);
        _tabletMain.SetActive(false);
    }

    public void ReturnToPauseMenu()
    {
        if (_controlsUI != null && _pauseUI != null)
        {
            _controlsUI.SetActive(false);
            _pauseUI.SetActive(true);
        }
        else
        {
            Debug.LogError("ControlsUI or PauseUI references are not assigned!");
        }
    }
}