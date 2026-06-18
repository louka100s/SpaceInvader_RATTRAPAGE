using UnityEngine;

public class UFO : MonoBehaviour
{
    public float speed = 3f;
    public int bonusPoints = 50;

    private int direction;
    private const float DespawnBound = 12f;

    /// <summary>
    /// Sets the horizontal travel direction. Call this immediately after instantiation.
    /// dir = 1 for left-to-right, dir = -1 for right-to-left.
    /// </summary>
    public void Launch(int dir)
    {
        direction = dir;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > DespawnBound)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Awards bonus points and destroys both the bullet and this UFO on contact with a player bullet.
    /// Score is handled here, not in Bullet.cs, to avoid double-counting.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(bonusPoints);
            }

            Destroy(gameObject);
        }
    }
}
