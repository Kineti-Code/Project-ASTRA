using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Finish finish;
    public GameObject GetSpeedrunTimer() => speedrunTimer;
    public GameObject speedrunTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameManager detected and destroyed.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        finish = FindFirstObjectByType<Finish>();

        SpeedrunTimer timerScript = FindFirstObjectByType<SpeedrunTimer>();
        if (timerScript != null) { speedrunTimer = timerScript.gameObject.transform.GetChild(0).gameObject; }
        GetSettingsFromPlayerPrefs();
    }

    public void GetSettingsFromPlayerPrefs()
    {
        if (finish != null)
        {
            finish.speedrunning = PlayerPrefs.GetInt("SkipCutscenes", 0) == 1;
        }

        if (speedrunTimer != null)
        {
            bool timerOn = PlayerPrefs.GetInt("Timer", 0) == 1;
            speedrunTimer.SetActive(timerOn);
        }
    }
}
