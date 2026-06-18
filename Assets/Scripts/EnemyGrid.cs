using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class EnemyGrid : MonoBehaviour
{
    public int rows = 5;
    public int columns = 10;
    public float enemySpacing = 1.2f;
    public float moveSpeed = 2f;
    public float dropDistance = 0.5f;
    public float bottomLimit = -4f;
    public GameObject enemyPrefab;
    public GameObject enemyBulletPrefab;
    public int currentLevel = 1;
    public Material tankMaterial;
    public Material normalEnemyMaterial;
    public Material diverMaterial;

    private int direction = 1;
    private float diveTimer = 0f;
    private float diveInterval = 4.5f;
    private bool diveEnabled = false;
    private bool needsDrop = false;
    private bool isInitialized = false;
    private bool justDropped = false;
    private float baseSpeed;
    private int totalEnemies;

    private void Start()
    {
        // Sync GameManager avec le niveau de cette scène pour robustesse
        // (évite le bug si la scène est lancée directement depuis l'éditeur)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLevel = currentLevel;
        }

        SpawnGrid();
        totalEnemies = transform.childCount;
        baseSpeed = moveSpeed;
        isInitialized = true;

        if (currentLevel >= 4)
        {
            diveEnabled = true;
        }
    }

    /// <summary>
    /// Spawns a rows x columns grid of enemies centered on this GameObject's position.
    /// At level 2, the first 2 enemies of each row are configured as shooters.
    /// </summary>
    private void SpawnGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float xPos = col * enemySpacing - (columns - 1) * enemySpacing / 2f;
                float yPos = -row * enemySpacing;
                Vector3 spawnPos = transform.position + new Vector3(xPos, yPos, 0f);

                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                enemy.transform.SetParent(transform);

                Enemy enemyScript = enemy.GetComponent<Enemy>();

                // --- TANKS : première ligne dès le Level 3 ---
                if (currentLevel >= 3 && row == 0)
                {
                    enemyScript.health = 2;
                    enemy.transform.localScale = new Vector3(1f, 1f, 1f);
                    Renderer r = enemy.GetComponent<Renderer>();
                    if (r != null && tankMaterial != null)
                    {
                        r.material = tankMaterial;
                    }
                }
                // --- TANKS : deuxième ligne au Level 4 ---
                else if (currentLevel >= 4 && row == 1)
                {
                    enemyScript.health = 2;
                    enemy.transform.localScale = new Vector3(1f, 1f, 1f);
                    Renderer r = enemy.GetComponent<Renderer>();
                    if (r != null && tankMaterial != null)
                    {
                        r.material = tankMaterial;
                    }
                }
                // --- DIVERS : dernière ligne au Level 4 ---
                else if (currentLevel >= 4 && row == rows - 1)
                {
                    Renderer r = enemy.GetComponent<Renderer>();
                    if (r != null && diverMaterial != null)
                    {
                        r.material = diverMaterial;
                    }
                    enemy.name = "Diver_" + col;
                }
                // --- ENNEMIS NORMAUX ---
                else
                {
                    Renderer r = enemy.GetComponent<Renderer>();
                    if (r != null && normalEnemyMaterial != null)
                    {
                        r.material = normalEnemyMaterial;
                    }
                }

                // --- TIR : logique progressive par niveau ---
                bool shouldShoot = false;

                if (currentLevel == 2)
                {
                    shouldShoot = (col < 2);
                }
                else if (currentLevel >= 3)
                {
                    if (currentLevel >= 4)
                    {
                        // Level 4 : tous les ennemis tirent
                        shouldShoot = true;
                    }
                    else
                    {
                        // Level 3 : toute la dernière ligne + les 2 premiers de chaque ligne
                        shouldShoot = (row == rows - 1) || (col < 2);
                    }
                }

                if (shouldShoot)
                {
                    enemyScript.isShooter = true;
                    enemyScript.enemyBulletPrefab = enemyBulletPrefab;
                }
            }
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        // 1. Victoire — toujours vérifier en premier
        if (transform.childCount == 0)
        {
            isInitialized = false;
            Debug.Log("All enemies destroyed. Calling VictorySequence.");
            StartCoroutine(VictorySequence());
            return;
        }

        // 2. Drop si nécessaire
        if (needsDrop)
        {
            transform.position += Vector3.down * dropDistance;
            direction *= -1;
            needsDrop = false;
            justDropped = true;
            CheckBottomReached();
            return;
        }

        // 3. Accélération progressive
        int remainingEnemies = transform.childCount;
        if (remainingEnemies > 0 && totalEnemies > 0)
        {
            float ratio = 1f - ((float)remainingEnemies / totalEnemies);
            moveSpeed = baseSpeed + (ratio * baseSpeed * 3f);
        }

        // 3b. Danger Flash — clignotement rouge quand il reste 5 ennemis ou moins
        if (remainingEnemies > 0 && remainingEnemies <= 5)
        {
            float blink = Mathf.PingPong(Time.time * 4f, 1f);
            foreach (Transform child in transform)
            {
                Renderer r = child.GetComponent<Renderer>();
                if (r != null)
                {
                    Color original = r.material.color;
                    r.material.color = Color.Lerp(original, Color.red, blink);
                }
            }
        }

        // 4. Mouvement horizontal
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        // 5. Lancer un Diver périodiquement (Level 4+)
        if (diveEnabled && transform.childCount > 0)
        {
            diveTimer += Time.deltaTime;
            if (diveTimer >= diveInterval)
            {
                diveTimer = 0f;
                TryLaunchDiver();
            }
        }

        // 6. Vérification des bords avec clamp anti-cascade
        if (justDropped)
        {
            justDropped = false;
        }
        else
        {
            foreach (Transform child in transform)
            {
                Vector3 viewportPos = Camera.main.WorldToViewportPoint(child.position);
                if (viewportPos.x >= 1f || viewportPos.x <= 0f)
                {
                    needsDrop = true;

                    float overshoot;
                    if (viewportPos.x >= 1f)
                    {
                        Vector3 borderWorld = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
                        overshoot = child.position.x - borderWorld.x;
                    }
                    else
                    {
                        Vector3 borderWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
                        overshoot = child.position.x - borderWorld.x;
                    }
                    transform.position -= new Vector3(overshoot, 0f, 0f);

                    break;
                }
            }
        }

        // 6. Vérification défaite — toujours en dernier
        CheckBottomReached();
    }

    /// <summary>
    /// Picks a Diver-named enemy (or any surviving enemy as fallback) and launches it
    /// toward the player's current position.
    /// </summary>
    private void TryLaunchDiver()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Collect children whose name starts with "Diver_"
        System.Collections.Generic.List<Transform> divers = new System.Collections.Generic.List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Diver_"))
            {
                divers.Add(child);
            }
        }

        // Fallback supprimé : si plus aucun Diver désigné, aucune plongée ne se déclenche
        if (divers.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, divers.Count);
        Transform diverTransform = divers[index];
        Diver diverScript = diverTransform.gameObject.AddComponent<Diver>();
        diverScript.Launch(player.transform.position);
    }

    /// <summary>
    /// Displays a brief victory message then triggers the level transition via GameManager.
    /// </summary>
    private IEnumerator VictorySequence()
    {
        isInitialized = false;

        // Bark de victoire selon le niveau
        if (BarkManager.Instance != null)
        {
            if (currentLevel == 1)
            {
                BarkManager.Instance.ShowBark("Première vague éliminée. Mais c'était l'échauffement...", 3f);
            }
            else if (currentLevel == 2)
            {
                BarkManager.Instance.ShowBark("Impressionnant. Mais nos radars détectent du lourd...", 3f);
            }
            else if (currentLevel == 3)
            {
                BarkManager.Instance.ShowBark("Soldat, vous êtes une légende. Mais le pire arrive.", 3f);
            }
            else if (currentLevel == 4)
            {
                BarkManager.Instance.ShowBark("Soldat. Je retire tout ce que j'ai dit.", 3f);
                yield return new WaitForSeconds(4f);
                BarkManager.Instance.ShowBark("Vous êtes le meilleur pilote que j'ai jamais vu.", 3f);
            }
        }

        // Afficher le texte de victoire
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            GameObject victoryText = new GameObject("VictoryText");
            victoryText.transform.SetParent(canvas.transform, false);

            Text text = victoryText.AddComponent<Text>();

            if (GameManager.Instance != null && GameManager.Instance.currentLevel >= 4)
            {
                text.text = "VICTOIRE !";
            }
            else
            {
                text.text = "NIVEAU TERMINÉ !";
            }

            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 50;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            RectTransform rt = victoryText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        yield return new WaitForSeconds(3f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LevelComplete();
        }
    }

    /// <summary>
    /// Checks if any enemy has reached the bottom of the screen and triggers Game Over.
    /// </summary>
    private void CheckBottomReached()
    {
        foreach (Transform child in transform)
        {
            if (child.position.y <= bottomLimit)
            {
                Debug.Log("Enemy reached bottom at Y = " + child.position.y + " (limit = " + bottomLimit + ")");
                GameManager.Instance.GameOver();
                return;
            }
        }
    }
}
