using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{

    public static PlayerPrefsManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    public void DeletePlayerPrefs() // called in javascript
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit() // for unity inspector
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
