using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{

    [SerializeField] private Button[] levelButtons;
    [SerializeField] private Sprite[] levelButtonSprites;
    [SerializeField] private Button goButton;

    private Image[] buttonSourceImages;
    private int selectedLevel = 0;
    private AudioManager audioManager;

    private void Start()
    {
        buttonSourceImages = new Image[levelButtons.Length];
        for (int i = 0; i < levelButtons.Length; i++)
        {
            buttonSourceImages[i] = levelButtons[i].GetComponent<Image>();
        }

        UpdateUnlockedLevels();
        Debug.Log("start sequence completed");
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void UpdateUnlockedLevels()
    {
        // First level is always unlocked
        levelButtons[0].gameObject.SetActive(true);

        // Unlock subsequent levels based on progress
        for (int i = 1; i < levelButtons.Length; i++)
        {
            bool isUnlocked = PlayerPrefs.GetInt("Level" + i, 0) == 1;
            levelButtons[i].gameObject.SetActive(isUnlocked);
        }
    }

    public void LevelLocked()
    {
        audioManager.PlaySFX(audioManager.lockedButtonClick);
    }

    public void SelectLevel(int level)
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        selectedLevel = level;
        levelButtons[level - 4].interactable = false;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i != level - 4)
            {
                buttonSourceImages[i].sprite = levelButtonSprites[i];
                levelButtons[i].interactable = true;
            }
        }

        goButton.interactable = true;
    }

    public void GoToLevel()
    {
        SceneManager.LoadScene(selectedLevel);
    }

    public void TestCoroutine()
    {
        Debug.Log("using coroutine");
        StartCoroutine(OpenSelectedLevel());
    }

    private IEnumerator OpenSelectedLevel()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(selectedLevel);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void OpenArtifactDrawer()
    {
        SceneManager.LoadScene(2);
    }

    public void ReturnToHome()
    {
        SceneManager.LoadScene(0);
    }
}
