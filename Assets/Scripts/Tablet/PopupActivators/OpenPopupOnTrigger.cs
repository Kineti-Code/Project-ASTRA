using UnityEngine;

public class OpenPopupOnTrigger : MonoBehaviour
{
    [System.Serializable]
    public class MessageData
    {
        public string title;
        [TextArea(3, 10)] public string[] pages;
    }

    [Header("Required components")]
    [SerializeField] private MessageData messageData;
    [SerializeField] private PopupMessenger popupMessenger;

    [Header("Extras")]
    [SerializeField] private bool skipQueue = false;
    private bool alreadyActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !alreadyActivated)
        {

            if (skipQueue)
            {
                popupMessenger.OpenTabletMessage(messageData.title, messageData.pages);
            }

            else
            {
                popupMessenger.QueueTabletMessage(messageData.title, messageData.pages);
            }

            alreadyActivated = true;
        }
    }
}