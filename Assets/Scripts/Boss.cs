using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Boss : MonoBehaviour
{
    public int maxHealth = 15;
    public float moveSpeed = 4.5f;
    public float shootInterval = 0.8f;
    public float boomerangIntervalMin = 9f;
    public float boomerangIntervalMax = 14f;
    public GameObject enemyBulletPrefab;

    public int currentHealth;
    private int direction = 1;
    private const float MinX = -7f;
    private const float MaxX = 7f;
    private Renderer bossRenderer;
    private Color baseColor = new Color(0.6f, 0f, 0.8f); // Violet
    private Transform firePoint;

    // Barre de vie UI
    private Slider healthBar;
    private GameObject healthBarObj;

    private void Start()
    {
        // Level 5 — sync GameManager pour robustesse si scène lancée directement
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLevel = 5;
        }

        currentHealth = maxHealth;
        bossRenderer = GetComponent<Renderer>();
        bossRenderer.material.color = baseColor;

        // Créer le firePoint
        GameObject fp = new GameObject("BossFirePoint");
        fp.transform.SetParent(transform);
        fp.transform.localPosition = new Vector3(0f, -1f, 0f);
        firePoint = fp.transform;

        CreateHealthBar();
        StartCoroutine(ShootLoop());
        StartCoroutine(BoomerangLoop());
    }

    private void Update()
    {
        // Mouvement horizontal avec rebond
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        if (transform.position.x >= MaxX)
        {
            direction = -1;
            transform.position = new Vector3(MaxX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= MinX)
        {
            direction = 1;
            transform.position = new Vector3(MinX, transform.position.y, transform.position.z);
        }

        // Clignotement danger sous 3 HP
        if (currentHealth <= 3 && currentHealth > 0)
        {
            float blink = Mathf.PingPong(Time.time * 6f, 1f);
            bossRenderer.material.color = Color.Lerp(Color.red, Color.white, blink);
        }
    }

    private IEnumerator ShootLoop()
    {
        yield return new WaitForSeconds(2f); // Délai avant le premier tir

        while (currentHealth > 0)
        {
            if (enemyBulletPrefab != null && firePoint != null)
            {
                Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(shootInterval);
        }
    }

    /// <summary>
    /// Fires a bouncing boomerang at random intervals, independently of normal shots.
    /// </summary>
    private IEnumerator BoomerangLoop()
    {
        // Première frappe après un délai initial aléatoire
        yield return new WaitForSeconds(Random.Range(boomerangIntervalMin, boomerangIntervalMax));

        while (currentHealth > 0)
        {
            SpawnBoomerang();
            yield return new WaitForSeconds(Random.Range(boomerangIntervalMin, boomerangIntervalMax));
        }
    }

    /// <summary>
    /// Creates a boomerang projectile that bounces horizontally across the screen.
    /// </summary>
    private void SpawnBoomerang()
    {
        if (firePoint == null) return;

        GameObject boomGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boomGO.name = "Boomerang";
        boomGO.tag = "EnemyBullet";
        boomGO.transform.position = firePoint.position;
        boomGO.transform.localScale = new Vector3(0.55f, 0.12f, 0.12f);

        // Matériau orange vif via URP Unlit
        Renderer r = boomGO.GetComponent<Renderer>();
        if (r != null)
        {
            Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
            Material mat = unlitShader != null
                ? new Material(unlitShader)
                : new Material(r.sharedMaterial);
            mat.color = new Color(1f, 0.45f, 0f); // Orange
            r.material = mat;
        }

        // Trigger collider
        BoxCollider col = boomGO.GetComponent<BoxCollider>();
        if (col != null) col.isTrigger = true;

        // Rigidbody cinématique
        Rigidbody rboom = boomGO.AddComponent<Rigidbody>();
        rboom.isKinematic = true;
        rboom.useGravity = false;

        // Direction aléatoire gauche/droite
        int boomDir = Random.value > 0.5f ? 1 : -1;
        Boomerang boom = boomGO.AddComponent<Boomerang>();
        boom.Launch(boomDir);
    }

    /// <summary>
    /// Applies one point of damage to the Boss. Updates health bar, color, and triggers victory at 0 HP.
    /// </summary>
    public void TakeDamage()
    {
        currentHealth--;

        // Mise à jour barre de vie
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }

        // Changement de couleur progressif violet → rouge
        if (currentHealth > 3)
        {
            float ratio = (float)currentHealth / maxHealth;
            bossRenderer.material.color = Color.Lerp(Color.red, baseColor, ratio);
        }

        // Flash blanc au hit
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            KillFlash.Spawn(transform.position);

            if (BarkManager.Instance != null)
            {
                BarkManager.Instance.ShowBark("LEUR CHEF EST TOMBÉ ! LA TERRE EST SAUVÉE !", 3f);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(50);
                GameManager.Instance.LevelComplete();
            }

            if (healthBarObj != null) Destroy(healthBarObj);
            Destroy(gameObject);
        }
    }

    private IEnumerator HitFlash()
    {
        bossRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.08f);

        if (currentHealth > 3)
        {
            float ratio = (float)currentHealth / maxHealth;
            bossRenderer.material.color = Color.Lerp(Color.red, baseColor, ratio);
        }
    }

    /// <summary>
    /// Dynamically creates a health bar Slider anchored at the top-center of the Canvas.
    /// </summary>
    private void CreateHealthBar()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        healthBarObj = new GameObject("BossHealthBar");
        healthBarObj.transform.SetParent(canvas.transform, false);

        RectTransform barRT = healthBarObj.AddComponent<RectTransform>();
        barRT.anchorMin = new Vector2(0.25f, 0.92f);
        barRT.anchorMax = new Vector2(0.75f, 0.96f);
        barRT.offsetMin = Vector2.zero;
        barRT.offsetMax = Vector2.zero;

        // Fond de la barre
        Image bgImage = healthBarObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        healthBar = healthBarObj.AddComponent<Slider>();
        healthBar.minValue = 0f;
        healthBar.maxValue = 1f;
        healthBar.value = 1f;
        healthBar.interactable = false;

        // Zone de remplissage
        GameObject fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(healthBarObj.transform, false);
        RectTransform fillAreaRT = fillArea.AddComponent<RectTransform>();
        fillAreaRT.anchorMin = Vector2.zero;
        fillAreaRT.anchorMax = Vector2.one;
        fillAreaRT.offsetMin = Vector2.zero;
        fillAreaRT.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.8f, 0f, 1f);

        healthBar.fillRect = fillRT;
        healthBar.targetGraphic = fillImage;
    }
}
