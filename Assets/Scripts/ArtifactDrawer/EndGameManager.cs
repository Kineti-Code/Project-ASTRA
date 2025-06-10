using System.Collections;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private CreditsManager creditsManager;
    [SerializeField] private OpenPopupOnEnable popupOnEnable;
    [SerializeField] private GameObject tabletHandle;
    [SerializeField] private GameObject tabletPopupUI;
    [SerializeField] private GameObject tabletCreditsUI;

    [Header("Editable Parameters")]
    [SerializeField] private int tabletEndPosition = 88;
    [SerializeField] private float tabletMovementSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.GetInt("Game Completed") == 1)
        {
            ManageEndGame();
        }
        // otherwise don't do anything
    }

    private void ManageEndGame()
    {
        popupOnEnable.enabled = true;
    }
    public void QueueCredits()
    {
        StartCoroutine(MoveTablet());
    }

    private IEnumerator MoveTablet()
    {
        Time.timeScale = 1;
        ShowCredits();
        while (tabletHandle.transform.localPosition.x < tabletEndPosition)
        {
            float step = tabletMovementSpeed * Time.unscaledDeltaTime;
            tabletHandle.transform.localPosition = Vector3.MoveTowards(tabletHandle.transform.localPosition, new Vector3(tabletEndPosition, 0, 0), step);
            yield return null; // Wait for next frame
        }
        StartCoroutine(creditsManager.ScrollDownCredits());
    }

    private void ShowCredits()
    {
        tabletPopupUI.SetActive(false);
        tabletCreditsUI.SetActive(true);
    }
}
