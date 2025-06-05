using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector2 _startPosition;
    [SerializeField]
    private Vector2 _endPosition;
    [SerializeField]
    private float _speed;
    private bool _movingToEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosition = transform.position;
        _movingToEnd = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_movingToEnd)
        {
            transform.position = Vector2.MoveTowards(transform.position, _endPosition, _speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, _endPosition) < 0.1f)
            {
                _movingToEnd = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _startPosition, _speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, _startPosition) < 0.1f)
            {
                _movingToEnd = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
