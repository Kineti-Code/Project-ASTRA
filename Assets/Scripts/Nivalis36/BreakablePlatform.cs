using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{

    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private float activationTime = 5f;
    [SerializeField] private float respawnTime = 10f;
    private float intervalTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    IEnumerator StartBreaking()
    {
        yield return new WaitForSeconds(intervalTime);
        Debug.Log(activationTime - intervalTime +  " seconds left");
        // add custom logic here

        yield return new WaitForSeconds(intervalTime);
        Debug.Log(activationTime - (intervalTime * 2) + " seconds left");
        // add custom logic here

        yield return new WaitForSeconds(intervalTime);
        Debug.Log(activationTime - (intervalTime * 3) + " seconds left");
        // add custom logic here

        yield return new WaitForSeconds(intervalTime);
        Debug.Log("ice breaking!!!");

        // add custom logic here

        StartCoroutine(PlatformCooldown());
        _boxCollider.enabled = false;
        _spriteRenderer.enabled = false;
    }

    IEnumerator PlatformCooldown()
    {
        yield return new WaitForSeconds(respawnTime);
        EnablePlatform();
    }

    public void EnablePlatform()
    {
        _boxCollider.enabled = true;
        _spriteRenderer.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && _boxCollider != null && _spriteRenderer != null)
        {
            intervalTime = activationTime / 4;
            StartCoroutine(StartBreaking());
        }

        else if (_boxCollider == null)
        {
            Debug.LogError("the boxcollider of a breakable platform is not defined");
        }

        else if (_spriteRenderer == null) 
        {
            Debug.LogError("the spriterenderer of a breakable platform is not defined");
        }
    }
}
