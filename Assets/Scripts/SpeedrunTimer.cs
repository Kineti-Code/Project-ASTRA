using UnityEngine;
using UnityEngine.UI;

public class SpeedrunTimer : MonoBehaviour
{
    [SerializeField] private Text uiText;

    public string formattedTime { get { return GetFormattedTime(); } }
    private float timer = 0f;
    private bool isPaused = false;

    void Update()
    {
        if (!isPaused)
        {
            timer += Time.deltaTime;

            if (uiText != null)
                uiText.text = formattedTime;
        }
    }

    public float GetRawTime()
    {
        return timer;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        int milliseconds = Mathf.FloorToInt((timer * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
