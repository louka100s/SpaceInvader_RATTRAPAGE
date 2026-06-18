using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    private const float TopBound = 10f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector3.up * speed * Time.fixedDeltaTime);

        if (rb.position.y > TopBound)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Detects collision with enemies and UFOs.
    /// Calls TakeDamage on Enemy — the script decides whether to die or survive (Tanks).
    /// Score is added per hit, not per kill: a Tank gives 2 points total (1 per shot).
    /// UFO score is handled by UFO.cs to avoid double-counting.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            KillFlash.Spawn(other.transform.position);

            Boss bossScript = other.GetComponent<Boss>();
            if (bossScript != null)
            {
                bossScript.TakeDamage();
                Destroy(gameObject);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(1);
                }

                if (ComboManager.Instance != null)
                {
                    ComboManager.Instance.RegisterKill();
                }

                ScorePopup.Spawn(other.transform.position, 1);
                return;
            }

            Enemy enemyScript = other.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage();
            }
            else
            {
                Destroy(other.gameObject);
            }

            Destroy(gameObject);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1);
            }

            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.RegisterKill();
            }

            ScorePopup.Spawn(other.transform.position, 1);
        }
        else if (other.CompareTag("UFO"))
        {
            KillFlash.Spawn(other.transform.position);
            ScorePopup.Spawn(other.transform.position, 50);

            Destroy(other.gameObject);
            Destroy(gameObject);

            if (ComboManager.Instance != null)
            {
                ComboManager.Instance.RegisterKill();
            }
        }
    }
}
