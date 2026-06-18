using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public bool isShooter = false;
    public GameObject enemyBulletPrefab;
    public float shootInterval = 3f;
    public float shootChance = 0.3f;

    /// <summary>
    /// Hit points for this enemy. Tanks are spawned with 2 HP by EnemyGrid.
    /// </summary>
    public int health = 1;

    private Transform firePoint;
    private Renderer enemyRenderer;
    private Color originalColor;

    // Partagé entre tous les ennemis pour ne déclencher le bark qu'une seule fois par niveau.
    private static bool tankHitBarkDone = false;

    private void Start()
    {
        tankHitBarkDone = false; // Reset à chaque spawn (premier ennemi spawné réinitialise le flag)

        GameObject firePointObject = new GameObject("EnemyFirePoint");
        firePointObject.transform.SetParent(transform);
        firePointObject.transform.localPosition = new Vector3(0f, -0.6f, 0f);
        firePoint = firePointObject.transform;

        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }

        if (isShooter)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    /// <summary>
    /// Applies one point of damage. Destroys the enemy when health reaches zero.
    /// Tanks flash white on hit to signal they survived. Triggers a bark on first tank hit.
    /// </summary>
    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            if (Random.value <= 0.08f)
            {
                GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pickup.name = "SpeedPickup";
                pickup.transform.position = transform.position;
                pickup.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                pickup.GetComponent<Renderer>().material.color = Color.yellow;
                pickup.GetComponent<BoxCollider>().isTrigger = true;
                Rigidbody rb = pickup.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                pickup.AddComponent<SpeedPickup>();
            }

            Destroy(gameObject);
        }
        else
        {
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = Color.Lerp(originalColor, Color.white, 0.5f);
            }

            if (!tankHitBarkDone && BarkManager.Instance != null)
            {
                tankHitBarkDone = true;
                BarkManager.Instance.ShowBark("Il est blindé ! Remettez-en une couche !", 2.5f);
            }
        }
    }

    /// <summary>
    /// Attempts to fire an EnemyBullet every shootInterval seconds with a shootChance probability.
    /// Runs until the GameObject is destroyed.
    /// </summary>
    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);

            if (Random.value <= shootChance && enemyBulletPrefab != null)
            {
                Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
            }
        }
    }
}
