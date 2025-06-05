using UnityEngine;

public class OpenPopupOnEnable : MonoBehaviour
{
    [Header("Message settings")]
    [SerializeField] private string messageHeader;
    [SerializeField, TextArea(3, 10)] private string[] messageBody;

    [Header("Required components")]
    [SerializeField] private PopupMessenger popupMessenger;

    [Header("Extra settings")]
    [SerializeField] private bool AlwaysShowPopup = true;
    [SerializeField] private string playerPrefsKey = null;
    [SerializeField] private bool skipQueue = false;
    [SerializeField] private float activationDelay = 0f;

    void Start()
    {
        Invoke(nameof(ActivatePopup), activationDelay);
    }

    private void ActivatePopup()
    {
        if (!AlwaysShowPopup && PlayerPrefs.GetInt(playerPrefsKey) == 0)
        {
            if (skipQueue)
            {
                popupMessenger.OpenTabletMessage(messageHeader, messageBody);
            }

            else
            {
                popupMessenger.QueueTabletMessage(messageHeader, messageBody);
            }

            PlayerPrefs.SetInt(playerPrefsKey, 1);
        }

        else if (AlwaysShowPopup)
        {

            if (skipQueue)
            {
                popupMessenger.OpenTabletMessage(messageHeader, messageBody);
            }

            else
            {
                popupMessenger.QueueTabletMessage(messageHeader, messageBody);
            }
        }
    }
}
