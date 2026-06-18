using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f;

    private const float BottomBound = -6f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector3.down * speed * Time.fixedDeltaTime);

        if (rb.position.y < BottomBound)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Detects collision with the player and destroys this projectile.
    /// Game Over logic is handled by PlayerController.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
