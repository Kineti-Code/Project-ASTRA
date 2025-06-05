using System.Runtime.CompilerServices;
using UnityEngine;
using static OpenPopupOnTrigger;

public class OpenPopupOnCollision : MonoBehaviour
{
    [Header("Text settings")]
    [SerializeField] string messageTitle;
    [SerializeField, TextArea(3, 10)] private string[] messageContent;

    [Header("Required components")]
    [SerializeField] private PopupMessenger popupMessenger;

    [Header("Extra settings")]
    [SerializeField] private bool skipQueue = false;

    private bool alreadyActivated = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !alreadyActivated)
        {

            if (skipQueue)
            {
                popupMessenger.OpenTabletMessage(messageTitle, messageContent);
            }

            else
            {
                popupMessenger.QueueTabletMessage(messageTitle, messageContent);
            }

            alreadyActivated = true;
        }
    }
}
