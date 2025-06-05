using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteManager : MonoBehaviour
{
    [SerializeField] private GameObject _tabletMain;
    [SerializeField] private GameObject _levelCompleteUI;
    [SerializeField] private Text _completionTimeText;
    public string completionTime;
    private int SceneID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0f;
        SceneID = SceneManager.GetActiveScene().buildIndex;
        UpdateCompletionTime();
    }

    private void UpdateCompletionTime()
    {
        if (_completionTimeText != null)
        {
            completionTime = GameManager.Instance.GetSpeedrunTimer().GetComponentInParent<SpeedrunTimer>().formattedTime;
            _completionTimeText.text = completionTime;
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneID);
        TurnOffTablet();
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
        _levelCompleteUI.SetActive(false);
        _tabletMain.SetActive(false);
        Time.timeScale = 1;
    }
}
