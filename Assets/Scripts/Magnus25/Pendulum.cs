using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [SerializeField] private float speed = 1f; // Speed of the pendulum swing
    [SerializeField] private float maxAngle = 45f; // Max angle the pendulum can swing
    [SerializeField, Range(-1, 1)] private int startDirection = 1;

    private float timer = 0f;

    void FixedUpdate()
    {
        // Calculate the angle using a sine function, which gives smooth oscillation
        float angle = Mathf.Sin(startDirection * timer * speed) * maxAngle;

        // Apply the calculated angle to the rotation of the pendulum
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Increment the timer to drive the oscillation
        timer += Time.fixedDeltaTime;
    }
}
