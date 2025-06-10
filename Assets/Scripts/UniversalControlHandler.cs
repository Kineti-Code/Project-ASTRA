using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UniversalControlHandler : MonoBehaviour
{
    [Header("General components")]
    [SerializeField] private GameObject _tabletMain;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private Button tabletButton;
    [SerializeField] private PopupMessenger popupMessenger;
    [SerializeField] private GameObject tabletHighlight;
    [HideInInspector] public bool pauseDisabled = false;

    [Header("Tutorial specific components")]
    [SerializeField] private EnableSliding enableSliding;
    [SerializeField] private GameObject _controlsUI;

    [Header("Terus1 specific components")]
    public Flashlight flashlight;

    [Header("Magnus25 specific components")]
    [SerializeField] private GameObject storm;
    private Storm stormScript;
    private int currentSceneIndex;

    private BreakablePlatform[] breakablePlatforms; // Nivalis36

    private void Start()
    {
        if (storm != null)
        {
            stormScript = storm.GetComponent<Storm>();
        }

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 5)
        {
            breakablePlatforms = FindObjectsByType<BreakablePlatform>(FindObjectsSortMode.None);
        }
    }

    private void TestLevels()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene(7);
        }
    }
    
    public void OnTabletButtonPress()
    {
        if (popupMessenger.popupQueued)
        {
            if (tabletHighlight.activeSelf)
            {
                tabletHighlight.SetActive(false);
            }
            popupMessenger.OpenTabletMessage(null, null);
        }

        else if (!pauseDisabled)
        {
            OpenPauseMenu();
        }
    }

    public void ReloadLevel()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach (Player player in players)
        {
            player.Respawn();
        }

        if (currentSceneIndex == 6 && enableSliding.numPlayersTriggered != PlayerManager.Instance.NumOfPlayers)
        {
            foreach (Player player in players)
            {
                player.inTutorialMode = true;
            }

            enableSliding.numPlayersTriggered = 0;
        }

        if (currentSceneIndex == 5)
        {
            foreach (BreakablePlatform platform in breakablePlatforms)
            {
                platform.EnablePlatform();
                platform.StopAllCoroutines();
            }
        }

        if (flashlight != null) // If this gameobject is in terus1
        {
            flashlight.RechargeFlashlight();
        }
        
        if (storm != null) // If this gameobject is in Magnus25
        {
            stormScript.ReactivateBunkers();
            stormScript.Reset();
            storm.SetActive(false);
        }
    }

    public void OpenPauseMenu()
    {
        if (_tabletMain != null && _pauseUI != null && !_tabletMain.activeSelf)
        {
            _tabletMain.SetActive(true);
            _pauseUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void OpenControls()
    {
        if (_tabletMain != null && !_tabletMain.activeSelf && _pauseUI != null)
        {
            _tabletMain.SetActive(true);
            _controlsUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            OpenPauseMenu();
        }

        TestLevels();
    }
}
