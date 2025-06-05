using UnityEngine;

public class GoLeftOrRight : MonoBehaviour
{

    [SerializeField] GameObject[] pointers;

    private void Start()
    {
        if (pointers != null)
        {
            foreach (GameObject pointer in pointers)
            {
                pointer.SetActive(false);
            }
        }

        else
        {
            Debug.LogError("Arrow not set stoopid");
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            foreach (GameObject pointer in pointers)
            {
                pointer.SetActive(true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            foreach (GameObject pointer in pointers)
            {
                pointer.SetActive(false);
            }
        }
    }
}
