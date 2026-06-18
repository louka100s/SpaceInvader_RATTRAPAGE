using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.3f;

    private float fireTimer = 0f;
    private bool isInvincible = false;
    private bool isDead = false;
    private const float MinX = -8f;
    private const float MaxX = 8f;
    private const float InvincibilityDuration = 2f;
    private const float BlinkInterval = 0.15f;

    private void Update()
    {
        HandleMovement();
        HandleFire();
    }

    /// <summary>
    /// Moves the player horizontally and clamps position within screen bounds.
    /// </summary>
    private void HandleMovement()
    {
        float moveInput = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            moveInput = -1f;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            moveInput = 1f;

        transform.Translate(Vector3.right * moveInput * speed * Time.deltaTime);

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(transform.position.x, MinX, MaxX);
        transform.position = clampedPosition;
    }

    /// <summary>
    /// Fires a bullet prefab from the firePoint when Space is pressed and cooldown has elapsed.
    /// </summary>
    private void HandleFire()
    {
        fireTimer -= Time.deltaTime;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && fireTimer <= 0f)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            fireTimer = fireCooldown;
        }
    }

    /// <summary>
    /// Detects collision with enemy bullets. Destroys the bullet, then either
    /// triggers invincibility if lives remain, or marks the player as dead.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Debug.Log("Player hit by: " + other.tag + " at " + other.transform.position);
            Destroy(other.gameObject);

            if (isDead || isInvincible) return;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();

                if (GameManager.Instance.lives <= 0)
                {
                    isDead = true;
                }
                else
                {
                    if (ScreenShake.Instance != null)
                    {
                        ScreenShake.Instance.Shake(0.25f, 0.2f);
                    }
                    StartCoroutine(InvincibilityCoroutine());
                }
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by: " + other.tag + " at " + other.transform.position);

            Destroy(other.gameObject);

            if (isDead || isInvincible) return;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();

                if (GameManager.Instance.lives <= 0)
                {
                    isDead = true;
                }
                else
                {
                    if (ScreenShake.Instance != null)
                    {
                        ScreenShake.Instance.Shake(0.25f, 0.2f);
                    }
                    StartCoroutine(InvincibilityCoroutine());
                }
            }
        }
    }

    /// <summary>
    /// Blinks the player renderer for InvincibilityDuration seconds, then
    /// re-enables it and clears the invincibility flag.
    /// </summary>
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        Renderer playerRenderer = GetComponent<Renderer>();
        float elapsed = 0f;

        while (elapsed < InvincibilityDuration)
        {
            if (playerRenderer != null)
                playerRenderer.enabled = !playerRenderer.enabled;

            yield return new WaitForSeconds(BlinkInterval);
            elapsed += BlinkInterval;
        }

        if (playerRenderer != null)
            playerRenderer.enabled = true;

        isInvincible = false;
    }
}
