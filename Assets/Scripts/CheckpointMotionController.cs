using UnityEngine;

public class CheckpointMotionController : MonoBehaviour
{

    private Vector3 _startPosition;
    private float _movementRange = 0.5f;
    private float _movementSpeed = 1f;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        float verticalOffset = Mathf.Sin(Time.time * _movementSpeed) * _movementRange;
        transform.position = _startPosition + new Vector3(0, verticalOffset, 0);
    }
}
