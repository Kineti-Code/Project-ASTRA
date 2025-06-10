using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private Scrollbar creditsBar;

    [Header("Adjustable parameters")]
    [SerializeField] private float scrollSpeed;

    private void OnEnable()
    {
        StartCoroutine(ScrollDownCredits());
    }

    private IEnumerator ScrollDownCredits()
    {
        creditsBar.value = 1;
        while (creditsBar.value > 0)
        {
            yield return null;
        }
    }
}
