using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{
    [SerializeField] private Vector2 velocityScale = new Vector2(1.3f, 2f);

    private GameObject player;
    private Rigidbody2D playerRb;
    private bool isPlayerAttached;

    void Start()
    {
        // Initialization if needed
    }

    void FixedUpdate()
    {
        if (isPlayerAttached)
        {
            playerRb.MovePosition(transform.position);
        }
        // Remove lastPosition update here
    }

    //void Update()
    //{
    //    if (isPlayerAttached && Input.GetKeyDown(KeyCode.W))
    //    {
    //        DetachPlayer();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isPlayerAttached)
        {
            AttachPlayer(collision);
        }
    }

    void AttachPlayer(Collider2D playerCollider)
    {
        player = playerCollider.gameObject;
        playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            isPlayerAttached = true;
            playerRb.linearVelocity = Vector2.zero;
            playerRb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void DetachPlayer()
    {
        if (playerRb != null)
        {
            StartCoroutine(CalculateAndApplyVelocity());
        }
    }

    private IEnumerator CalculateAndApplyVelocity()
    {
        isPlayerAttached = false;
        playerRb.bodyType = RigidbodyType2D.Dynamic;

        // Capture the initial position before waiting
        Vector2 initialPosition = transform.position;
        yield return new WaitForFixedUpdate(); // Wait for the next physics update

        // Get the position after the physics update
        Vector2 currentPosition = transform.position;
        Vector2 velocity = (currentPosition - initialPosition) / Time.fixedDeltaTime;

        ApplyVelocity(velocity);
    }

    private void ApplyVelocity(Vector2 velocity)
    {
        playerRb.linearVelocity = velocity * velocityScale;
        player = null;
        playerRb = null;
    }
}