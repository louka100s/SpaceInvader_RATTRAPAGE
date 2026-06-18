using UnityEngine;
using System.Collections;

public class BarkTrigger : MonoBehaviour
{
    public int currentLevel = 1;

    private bool firstKillTriggered = false;
    private bool fewEnemiesTriggered = false;
    private bool halfEnemiesTriggered = false;
    private bool firstLifeLostTriggered = false;
    private bool firstDiverTriggered = false;
    private bool firstDiverKilledTriggered = false;
    private bool playerReassureTriggered = false;
    private bool commanderRecoverTriggered = false;
    private bool ufoTriggered = false;
    private bool bossHalfTriggered = false;
    private bool bossLowTriggered = false;

    private int lastScore = 0;
    private int lastLives = 3;
    private int lastChildCount = 50;
    private int totalEnemies = 50;

    void Start()
    {
        StartCoroutine(IntroBark());

        EnemyGrid grid = FindObjectOfType<EnemyGrid>();
        if (grid != null)
        {
            totalEnemies = grid.rows * grid.columns;
            lastChildCount = totalEnemies;
        }

        if (GameManager.Instance != null)
        {
            lastScore = GameManager.Instance.score;
            lastLives = GameManager.Instance.lives;
        }
    }

    private IEnumerator IntroBark()
    {
        yield return new WaitForSeconds(1f);

        if (currentLevel == 1)
        {
            BarkManager.Instance.ShowBark("Soldat ! Flèches pour bouger, Espace pour tirer. Bonne chance.", 3.5f);
        }
        else if (currentLevel == 2)
        {
            BarkManager.Instance.ShowBark("Attention soldat, cette fois ils ripostent !", 3f);
            yield return new WaitForSeconds(4f);
            BarkManager.Instance.ShowBark("On a installé des boucliers. Utilisez-les à bon escient.", 3f);
        }
        else if (currentLevel == 3)
        {
            BarkManager.Instance.ShowBark("Alerte ! Les rouges encaissent deux tirs. Visez bien !", 3.5f);
        }
        else if (currentLevel == 4)
        {
            BarkManager.Instance.ShowBark("C'est pas possible... C'est la guerre des mondes là...", 3f);
            yield return new WaitForSeconds(4f);
            BarkManager.Instance.ShowBark("Ils sont kamikazes en plus ?! On va tous y passer !", 3f);
        }
        else if (currentLevel == 5)
        {
            BarkManager.Instance.ShowBark("C'est leur chef... Un contre un, soldat.", 3f);
            yield return new WaitForSeconds(4f);
            BarkManager.Instance.ShowBark("Visez entre les boucliers. Timing is everything.", 3f);
        }
    }

    void Update()
    {
        if (BarkManager.Instance == null) return;
        if (GameManager.Instance == null) return;

        int currentScore = GameManager.Instance.score;
        int currentLives = GameManager.Instance.lives;

        EnemyGrid grid = FindObjectOfType<EnemyGrid>();
        int currentChildCount = 0;
        if (grid != null)
        {
            currentChildCount = grid.transform.childCount;
        }

        // --- DÉTECTION : premier kill ---
        if (!firstKillTriggered && currentScore > lastScore && lastScore == 0)
        {
            firstKillTriggered = true;
            if (currentLevel == 1)
            {
                BarkManager.Instance.ShowBark("Bien visé ! Continuez à les dégommer !", 2.5f);
            }
        }

        // --- DÉTECTION : première vie perdue ---
        if (!firstLifeLostTriggered && currentLives < lastLives)
        {
            firstLifeLostTriggered = true;
            if (currentLevel <= 3)
            {
                BarkManager.Instance.ShowBark("Touché ! Il vous reste des vies, accrochez-vous !", 2.5f);
            }
            else if (currentLevel == 4)
            {
                BarkManager.Instance.ShowBark("On perd du monde ! C'est foutu...", 2.5f);
            }
        }

        // --- DÉTECTION : il reste la moitié des ennemis (Level 4 — joueur rassure) ---
        if (!playerReassureTriggered && currentLevel == 4 && currentChildCount <= totalEnemies / 2 && currentChildCount > 0)
        {
            playerReassureTriggered = true;
            BarkManager.Instance.ShowBark("Commandant, reprenez-vous. On gère.", 3f, true);
            StartCoroutine(CommanderRecovers());
        }

        // --- DÉTECTION : il reste 10 ennemis ---
        if (!fewEnemiesTriggered && currentChildCount <= 10 && currentChildCount > 0)
        {
            fewEnemiesTriggered = true;
            if (currentLevel == 1)
            {
                BarkManager.Instance.ShowBark("Ils accélèrent ! Restez concentré !", 2.5f);
            }
            else if (currentLevel == 4)
            {
                BarkManager.Instance.ShowBark("TOUS AVEC LE PILOTE ! ON LÂCHE RIEN !", 3f);
            }
            else
            {
                BarkManager.Instance.ShowBark("On y est presque ! Lâchez rien !", 2.5f);
            }
        }

        // --- DÉTECTION : UFO apparaît ---
        if (!ufoTriggered)
        {
            GameObject ufo = GameObject.Find("UFO");
            if (ufo != null)
            {
                ufoTriggered = true;
                BarkManager.Instance.ShowBark("Contact radar inconnu ! Abattez-le pour un gros bonus !", 3f);
            }
        }

        // --- DÉTECTION : Boss HP ---
        if (currentLevel == 5)
        {
            Boss boss = Object.FindFirstObjectByType<Boss>();
            if (boss != null)
            {
                float hpRatio = (float)boss.currentHealth / boss.maxHealth;

                if (!bossHalfTriggered && hpRatio <= 0.5f)
                {
                    bossHalfTriggered = true;
                    BarkManager.Instance.ShowBark("Il faiblit ! Continuez le pilonnage !", 2.5f);
                }

                if (!bossLowTriggered && hpRatio <= 0.2f)
                {
                    bossLowTriggered = true;
                    BarkManager.Instance.ShowBark("IL EST PRESQUE FINI ! ACHEVEZ-LE !", 2.5f);
                }
            }
        }

        lastScore = currentScore;
        lastLives = currentLives;
        lastChildCount = currentChildCount;
    }

    private IEnumerator CommanderRecovers()
    {
        yield return new WaitForSeconds(4f);
        if (!commanderRecoverTriggered)
        {
            commanderRecoverTriggered = true;
            BarkManager.Instance.ShowBark("...Vous avez raison. ALLEZ ON LES FINIT !", 3f);
        }
    }
}
