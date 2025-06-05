using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Main UI Elements")]
    [SerializeField] private GameObject _mainSettingsUI;
    [SerializeField] private GameObject _generalSettings;
    [SerializeField] private GameObject _gameSettings;
    [SerializeField] private GameObject _audioSettings;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private PauseManager _pauseManager;
    [SerializeField] private GameObject _tabletMain;
    [SerializeField] private GameObject _backToPauseMenu;

    [Header("Extra UI Elements")]
    [SerializeField] private SplitscreenManager _splitscreenManager;
    [SerializeField] private MinimapCameraController _minimapController;

    public List<GameObject> _activePlayers = new List<GameObject>();
    [SerializeField] private GameObject[] Players; // Assign in Inspector

    public int _numOfPlayers = 1;

    private Finish finish;
    private GameObject speedrunTimer;

    void OnEnable()
    {
        if (!_tabletMain.activeSelf)
        {
            _tabletMain.SetActive(true);
        }

        _mainSettingsUI.SetActive(true);
        changeMenu(PlayerPrefs.GetInt("SettingsMenu", 0));

        Time.timeScale = 0;

        _backToPauseMenu.SetActive(false);
        ShowPauseButton();
        LoadVolumes();
    }
    private void Start()
    {
        finish = FindFirstObjectByType<Finish>();
        speedrunTimer = GameManager.Instance?.GetSpeedrunTimer();
        UpdateSpeedrunSettings();
    }


    public void changeMenu(int menuNum)
    {
        switch (menuNum)
        {
            case 0:
                _generalSettings.SetActive(true);
                _gameSettings.SetActive(false);
                _audioSettings.SetActive(false);
                PlayerPrefs.SetInt("SettingsMenu", 0);
                break;
            case 1:
                _generalSettings.SetActive(false);
                _gameSettings.SetActive(true);
                _audioSettings.SetActive(false);
                PlayerPrefs.SetInt("SettingsMenu", 1);
                break;
            case 2:
                _generalSettings.SetActive(false);
                _gameSettings.SetActive(false);
                _audioSettings.SetActive(true);
                PlayerPrefs.SetInt("SettingsMenu", 2);
                break;
            default:
                _generalSettings.SetActive(true);
                _gameSettings.SetActive(false);
                _audioSettings.SetActive(false);
                break;
        }
    }

    public void ShowPauseButton()
    {
        if (_pauseManager.fromPauseMenu)
        {
            _backToPauseMenu.SetActive(true);
            _pauseManager.fromPauseMenu = false;
        }
    }

    public void ReturnToPauseMenu()
    {
        _mainSettingsUI.SetActive(false);
        _pauseUI.SetActive(true);
    }

    public void ExitSettings()
    {
        Time.timeScale = 1;
        _mainSettingsUI.SetActive(false);
        _tabletMain.SetActive(false);
    }

    /* General Settings Functions */

    public void LoadTutorial()
    {
        SceneManager.LoadScene(3);
    }

    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    /* Game Settings Functions */

    [Header("Game Settings References")]
    [SerializeField] private Toggle _timer;
    [SerializeField] private Toggle _skipCutscenes;
    private bool speedrunMode = false;

    private void UpdateSpeedrunSettings()
    {
        if (PlayerPrefs.GetInt("SkipCutscenes", 0) == 1) { _skipCutscenes.isOn = true; } else { _skipCutscenes.isOn = false; }
        if (PlayerPrefs.GetInt("Timer", 0) == 1) { _timer.isOn = true; } else { _timer.isOn = false; }
    }

    public void toggleSpeedrunMode()
    {
        speedrunMode = !speedrunMode;

        if (speedrunMode)
        {
            _timer.isOn = true;
            _skipCutscenes.isOn = true;
            toggleSkipCutscenes();
            toggleTimer();
        }

        else
        {
            _timer.isOn = false;
            _skipCutscenes.isOn = false;
            toggleSkipCutscenes();
            toggleTimer();
        }
    }

    public void toggleSkipCutscenes()
    {
        if (finish != null)
        {
            finish.speedrunning = _skipCutscenes.isOn;
            PlayerPrefs.SetInt("SkipCutscenes", finish.speedrunning ? 1 : 0);
        }

        else
        {
            _skipCutscenes.isOn = false;
        }
    }

    public void toggleTimer()
    {
        if (speedrunTimer != null)
        {
            speedrunTimer.SetActive(_timer.isOn);
            PlayerPrefs.SetInt("Timer", _timer.isOn ? 1 : 0);
        }

        else
        {
            _timer.isOn = false;
        }
    }

    public void UpdatePlayerCount(int numOfPlayers)
    {
        if (numOfPlayers != _numOfPlayers)
        {
            Debug.Log("Updating number of players to " + numOfPlayers);

            if (numOfPlayers > _numOfPlayers)
            {
                // Add new players
                for (int i = _numOfPlayers + 1; i <= numOfPlayers; i++)
                {
                    AddPlayer(i);
                }
            }
            else if (numOfPlayers < _numOfPlayers)
            {
                // Remove excess players
                for (int i = _numOfPlayers; i > numOfPlayers; i--)
                {
                    RemovePlayer(i);
                }
            }

            _numOfPlayers = numOfPlayers;
            PlayerManager.Instance.NumOfPlayers = _numOfPlayers;

            if (SceneManager.GetActiveScene().buildIndex != 0 || SceneManager.GetActiveScene().buildIndex != 3)
            {
                _splitscreenManager.AdjustSplitscreenConfig();
                _minimapController.UpdateNumOfPlayers();
            }

        }
        else
        {
            Debug.Log("No new players added");
        }
    }

    private void AddPlayer(int playerIndex)
    {
    
        // Activate existing player objects
        if (playerIndex - 2 >= 0 && playerIndex - 2 < Players.Length)
        {
            GameObject existingPlayer = Players[playerIndex - 2];
            if (existingPlayer != null)
            {
                existingPlayer.SetActive(true);
                _activePlayers.Add(existingPlayer);
            }
            else
            {
                Debug.LogError("Player reference at index " + (playerIndex - 2) + " is null!");
            }
        }
        else
        {
            Debug.LogError("Invalid player index for main menu: " + playerIndex);
        }
    
    }

    private void RemovePlayer(int playerIndex)
    {
        if (playerIndex - 2 < _activePlayers.Count && playerIndex - 2 >= 0)
        {
            GameObject playerToRemove = _activePlayers[playerIndex - 2];
            _activePlayers.RemoveAt(playerIndex - 2);

            playerToRemove.SetActive(false);
            Debug.Log("Player " + playerIndex + " deactivated");

        }
        else
        {
            Debug.LogWarning("No player to remove at index " + playerIndex);
        }
    }

    // Audio Settings Functions

    [Header("External Audio References")]
    [SerializeField] private AudioManager AudioManager;

    [Header("Audio Control Sliders")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _ambienceVolumeSlider;
    [SerializeField] private Slider _SFXVolumeSlider;

    public void changeVolume()
    {
        float masterVolume = _masterVolumeSlider.value;
        float musicVolume = _musicVolumeSlider.value;
        float ambienceVolume = _ambienceVolumeSlider.value;
        float sfxVolume = _SFXVolumeSlider.value;
        AudioManager.AdjustMusicVolume(masterVolume, new float[] { musicVolume, ambienceVolume, sfxVolume });
    }

    private void LoadVolumes()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        _masterVolumeSlider.value = masterVolume;
        _musicVolumeSlider.value = musicVolume;
        _ambienceVolumeSlider.value = ambienceVolume;
        _SFXVolumeSlider.value = sfxVolume;
    }

    public void ResetAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", 1f);
        PlayerPrefs.SetFloat("MusicVolume", 1f);
        PlayerPrefs.SetFloat("AmbienceVolume", 1f);
        PlayerPrefs.SetFloat("SFXVolume", 1f);
        LoadVolumes();
        changeVolume();
    }
}
