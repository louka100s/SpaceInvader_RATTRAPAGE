using UnityEngine;

public class Shield : MonoBehaviour
{
    /// <summary>
    /// Destroys the bullet and this shield block when either a player or enemy bullet makes contact.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet") || other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            // Block is destroyed on contact but the enemy continues descending
            Destroy(gameObject);
        }
    }
}
