using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private Scrollbar creditsBar;

    [Header("Adjustable parameters")]
    [SerializeField] private float scrollSpeed;

    public IEnumerator ScrollDownCredits()
    {
        creditsBar.value = 1;
        while (creditsBar.value > 0)
        {
            float step = scrollSpeed * Time.unscaledDeltaTime;
            creditsBar.value = Mathf.MoveTowards(creditsBar.value, 0, step);
            yield return null; // Wait for next frame
        }
        // Do something cool here ig
    }

    
}
