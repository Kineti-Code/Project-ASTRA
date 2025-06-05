using UnityEngine;

public class Artifact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.collectArtifact();
            }

            Destroy(this.gameObject);
        }
    }
}
