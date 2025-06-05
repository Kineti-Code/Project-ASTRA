using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessenger : MonoBehaviour
{

    [SerializeField] private PopupsManager popupController;
    [SerializeField] private GameObject popupUI;
    [SerializeField] private GameObject tabletMain;
    [SerializeField] private GameObject popupIndicator;
    [SerializeField] private GameObject tabletHighlight;
    private Coroutine highlightCoroutine;
    private string title;
    private string[] pages;
    public bool popupQueued = false;

    public void QueueTabletMessage(string title, string[] pages)
    {
        this.title = title;
        this.pages = pages;
        popupQueued = true;
        popupIndicator.SetActive(true);
        highlightCoroutine = StartCoroutine(HighlightPopup());
    }

    private IEnumerator HighlightPopup()
    {
        yield return new WaitForSeconds(3f);
        tabletHighlight.SetActive(true);
    }

    public void OpenTabletMessage(string provided_title, string[] provided_pages)
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
        }
        tabletHighlight.SetActive(false);
        popupIndicator.SetActive(false);
        popupQueued = false;
        Time.timeScale = 0f;
        tabletMain.SetActive(true);
        popupUI.SetActive(true);

        if (provided_title != null && provided_pages != null)
        {
            popupController.ShowMessage(provided_title, provided_pages);
        }
        else
        {
            popupController.ShowMessage(title, pages);
        }
    }
}
