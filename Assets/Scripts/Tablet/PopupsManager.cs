using UnityEngine;
using UnityEngine.UI;

public class PopupsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button doneButton;

    [Header("Tablet References")]
    [SerializeField] private GameObject tabletMain;
    [SerializeField] private GameObject popupUI;

    [Header("Messages")]
    private string[] currentPages;
    private int currentPageIndex;
    private string currentTitle;

    void Start()
    {
        // Initialize button listeners
        nextButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(PreviousPage);
        doneButton.onClick.AddListener(ClosePopup);
    }

    public void ShowMessage(string title, string[] pages)
    {
        currentTitle = title;
        currentPages = pages;
        currentPageIndex = 0;

        UpdateUI();
    }

    public void ExitUI()
    {
        Time.timeScale = 1f;
        tabletMain.SetActive(false);
        popupUI.SetActive(false);
    }

    private void UpdateUI()
    {
        titleText.text = currentTitle;
        bodyText.text = currentPages[currentPageIndex];

        // Button visibility logic
        backButton.gameObject.SetActive(currentPageIndex > 0);
        bool isFirstPage = currentPageIndex == 0;
        bool isLastPage = currentPageIndex >= currentPages.Length - 1;
        nextButton.gameObject.SetActive(!isLastPage);
        doneButton.gameObject.SetActive(isLastPage);

        if (isFirstPage)
        {
            nextButton.transform.localPosition = new Vector3(0, -150, 0);
        }

        else
        {
            nextButton.transform.localPosition = new Vector3(45, -150, 0);
        }

        if (isLastPage)
        {
            doneButton.transform.localPosition = new Vector3(45, -150, 0);
        }

        if (isFirstPage && isLastPage)
        {
            doneButton.transform.localPosition = new Vector3(0, -150, 0);
        }
    }

    private void NextPage()
    {
        currentPageIndex = Mathf.Clamp(currentPageIndex + 1, 0, currentPages.Length - 1);
        UpdateUI();
    }

    private void PreviousPage()
    {
        currentPageIndex = Mathf.Clamp(currentPageIndex - 1, 0, currentPages.Length - 1);
        UpdateUI();
    }

    private void ClosePopup()
    {
        currentPages = null;
        currentPageIndex = 0;
    }
}