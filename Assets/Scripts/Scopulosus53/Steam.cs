using UnityEngine;

public class Steam : MonoBehaviour
{
    private float _pushForce = 150f; // Adjustable upward force

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(Vector2.up * _pushForce, ForceMode2D.Force);

        }
    }
}
