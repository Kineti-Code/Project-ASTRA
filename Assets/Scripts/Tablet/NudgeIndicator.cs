using System.Collections;
using UnityEngine;

public class NudgeIndicator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float nudgeDistance = 10f; // How far the object moves down
    [SerializeField] private float nudgeDuration = 0.2f; // Duration of the nudge down
    [SerializeField] private float bounceBackDuration = 0.3f; // Duration of the bounce back
    [SerializeField] private float delayBetweenNudges = 5f; // Delay between nudges

    private RectTransform popupIndicatorTransform;
    private Vector3 originalPosition; // Stores the original position of the object

    void Start()
    {
        // Get the RectTransform of the parent UI element
        popupIndicatorTransform = GetComponentInParent<RectTransform>();

        // Store the original position of the object
        originalPosition = popupIndicatorTransform.localPosition;

        // Start the nudge coroutine
        StartCoroutine(GiveNudge());
    }

    private IEnumerator GiveNudge()
    {
        while (true) // Loop forever
        {
            // Wait for the delay between nudges
            yield return new WaitForSeconds(delayBetweenNudges);

            // Move the object down
            Vector3 targetPosition = originalPosition + Vector3.up * nudgeDistance;
            yield return MoveToPosition(targetPosition, nudgeDuration);

            // Move the object back up to the original position with a bounce effect
            yield return MoveToPosition(originalPosition, bounceBackDuration);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = popupIndicatorTransform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Smoothly interpolate between the start and target positions
            popupIndicatorTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the exact target position
        popupIndicatorTransform.localPosition = targetPosition;
    }
}