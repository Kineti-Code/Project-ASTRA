using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Tablet Components")]
    [SerializeField] private GameObject _tabletMain;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _controlsUI;

    [Header("Pause menu buttons")]
    [SerializeField] private GameObject _levelSelectButton;
    [SerializeField] private GameObject _homeButton;

    public bool fromPauseMenu = false; // indicates if a menu was activated from pause menu
    private int SceneID;

    void OnEnable()
    {
        if (!_tabletMain.activeSelf)
        {
            _tabletMain.SetActive(true);
        }

        //Time.timeScale = 0;
        SceneID = SceneManager.GetActiveScene().buildIndex;
        _pauseUI.SetActive(true);
        ChooseButtons();
    }

    private void ChooseButtons()
    {
        switch (SceneID)
        {
            case 0: // Main Menu
                _homeButton.SetActive(false);
                _levelSelectButton.transform.localPosition = new Vector3(0, -40, transform.localPosition.z);
                break;
            case 6: // Tutorial
                if (PlayerPrefs.GetInt("Tutorial progress") != 2)
                {
                    _homeButton.SetActive(false);
                    _levelSelectButton.SetActive(false);
                }
                break;
        }
    }

    public void Resume()
    {
        TurnOffTablet();
    }

    // Only works when tablet is initally off
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneID);
        TurnOffTablet();
    }

    public void OpenSettings()
    {
        fromPauseMenu = true;
        _pauseUI.SetActive(false);
        _settingsUI.SetActive(true);
    }

    public void OpenControls()
    {
        fromPauseMenu = true;
        _pauseUI.SetActive(false);
        _controlsUI.SetActive(true);
    }

    public void GoToLevelSelect()
    {
        TurnOffTablet();
        SceneManager.LoadScene(1);
    }

    public void ReturnToHome()
    {
        TurnOffTablet();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void TurnOffTablet()
    {
        _pauseUI.SetActive(false);
        _tabletMain.SetActive(false);
        Time.timeScale = 1;
    }
}
