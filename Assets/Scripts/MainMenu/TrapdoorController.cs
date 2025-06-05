using System.Collections;
using UnityEngine;

public class TrapdoorController : MonoBehaviour
{
    [SerializeField] private GameObject trapdoorLeft;
    [SerializeField] private GameObject trapdoorRight;
    [SerializeField] private float _speed = 2.49f;
    private AudioManager audioManager;

    // Target positions for opening and closing
    // Adjust these values as needed
    private float leftOpenX = 0.28f;
    private float leftClosedX = 2.18f;
    private float rightOpenX = 7.68f;
    private float rightClosedX = 5.78f;

    private void Start()
    {
        // Close the trapdoor at the start
        trapdoorLeft.transform.localPosition = new Vector3(leftClosedX, trapdoorLeft.transform.localPosition.y, trapdoorLeft.transform.localPosition.z);
        trapdoorRight.transform.localPosition = new Vector3(rightClosedX, trapdoorRight.transform.localPosition.y, trapdoorRight.transform.localPosition.z);
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public void OpenTrapdoor(bool isOpening)
    {
        StartCoroutine(AdjustTrapdoor(isOpening));
    }

    private IEnumerator AdjustTrapdoor(bool isOpening)
    {
        // Set the target x positions based on whether we're opening or closing
        float targetLeftX = isOpening ? leftOpenX : leftClosedX;
        float targetRightX = isOpening ? rightOpenX : rightClosedX;

        audioManager.PlaySFX(audioManager.trapdoorHydraulics);

        // Continue until both trapdoors reach their target positions
        while (!Mathf.Approximately(trapdoorLeft.transform.localPosition.x, targetLeftX) ||
               !Mathf.Approximately(trapdoorRight.transform.localPosition.x, targetRightX))
        {
            // Move left trapdoor toward target
            float newLeftX = Mathf.MoveTowards(trapdoorLeft.transform.localPosition.x, targetLeftX, _speed * Time.deltaTime);
            // Move right trapdoor toward target
            float newRightX = Mathf.MoveTowards(trapdoorRight.transform.localPosition.x, targetRightX, _speed * Time.deltaTime);

            // Update positions (keeping Y and Z unchanged)
            trapdoorLeft.transform.localPosition = new Vector3(newLeftX, trapdoorLeft.transform.localPosition.y, trapdoorLeft.transform.localPosition.z);
            trapdoorRight.transform.localPosition = new Vector3(newRightX, trapdoorRight.transform.localPosition.y, trapdoorRight.transform.localPosition.z);

            yield return null;
        }
    }
}