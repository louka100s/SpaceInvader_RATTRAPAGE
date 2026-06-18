using UnityEngine;

public class Diver : MonoBehaviour
{
    public float diveSpeed = 6f;

    private Vector3 diveDirection;
    private const float DespawnBound = 12f;

    // Un seul bark par niveau, partagé entre tous les Divers.
    private static bool diverBarkDone = false;

    private void Start()
    {
        diverBarkDone = false;
    }

    /// <summary>
    /// Detaches from the enemy grid and launches toward the given target position.
    /// Direction is fixed at launch — the diver does not track the player.
    /// Triggers a one-shot bark on the first dive of the level.
    /// </summary>
    public void Launch(Vector3 targetPosition)
    {
        // Detach from grid parent so the diver moves independently
        transform.SetParent(null);

        // Aim at the player's position at the moment of launch
        diveDirection = (targetPosition - transform.position).normalized;

        // Disable Enemy script to stop any active shooting coroutine
        Enemy enemyScript = GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.enabled = false;
        }

        if (!diverBarkDone && BarkManager.Instance != null)
        {
            diverBarkDone = true;
            BarkManager.Instance.ShowBark("IL FONCE SUR NOUS ! C'est foutu...", 2.5f);
        }
    }

    private void Update()
    {
        transform.Translate(diveDirection * diveSpeed * Time.deltaTime, Space.World);

        // Despawn if the diver leaves the play area
        if (transform.position.y < -7f || Mathf.Abs(transform.position.x) > DespawnBound)
        {
            Destroy(gameObject);
        }
    }
}
