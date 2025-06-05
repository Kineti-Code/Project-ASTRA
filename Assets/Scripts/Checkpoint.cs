using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.transform.GetComponent<Player>();
            _animator.SetTrigger("Activate");

            if (player != null)
            {
                player.updateCheckpoint(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2 + 1));
            }
        }
    }
}
