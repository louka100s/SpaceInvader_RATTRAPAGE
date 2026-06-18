using UnityEngine;

/// <summary>
/// Projectile spécial du Boss : se déplace horizontalement en rebondissant sur les bords,
/// dérive lentement vers le bas, et disparaît après sa durée de vie.
/// Tagué "EnemyBullet" — PlayerController et Shield.cs gèrent les collisions avec le joueur et les boucliers.
/// </summary>
public class Boomerang : MonoBehaviour
{
    private const float HorizontalSpeed = 5.5f;
    private const float VerticalDrift = 1.0f;
    private const float LeftBound = -8.8f;
    private const float RightBound = 8.8f;
    private const float Lifetime = 6f;
    private const float RotationSpeed = 700f;
    private const float ActivationDelay = 0.12f;

    private int horizontalDirection = 1;
    private Rigidbody rb;
    private float spawnTime;
    private bool isActive = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        spawnTime = Time.time;
        // Délai court pour éviter la collision immédiate avec le Boss au spawn
        Invoke(nameof(Activate), ActivationDelay);
    }

    private void Activate()
    {
        isActive = true;
    }

    /// <summary>
    /// Définit la direction horizontale initiale (+1 droite, -1 gauche).
    /// </summary>
    public void Launch(int direction)
    {
        horizontalDirection = direction;
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        Vector3 delta = new Vector3(horizontalDirection * HorizontalSpeed, -VerticalDrift, 0f) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + delta);

        // Rebond sur les bords latéraux
        Vector3 pos = rb.position;
        if (pos.x >= RightBound)
        {
            horizontalDirection = -1;
        }
        else if (pos.x <= LeftBound)
        {
            horizontalDirection = 1;
        }

        // Destruction si hors-écran ou durée de vie dépassée
        if (Time.time - spawnTime >= Lifetime || pos.y < -7f)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Rotation continue pour effet visuel "boomerang"
        transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
    }
}
