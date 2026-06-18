using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BarkManager : MonoBehaviour
{
    public static BarkManager Instance;

    private Text barkText;
    private Image barkBackground;
    private GameObject barkContainer;
    private Coroutine currentBark;
    private Queue<BarkData> barkQueue = new Queue<BarkData>();
    private bool isShowingBark = false;

    // Couleurs des bulles
    private Color commanderColor = new Color(0f, 0f, 0f);
    private Color playerColor = new Color(0f, 0f, 0f);
    private Color commanderBgColor = new Color(1f, 1f, 1f, 0.35f);
    private Color playerBgColor = new Color(0.7f, 1f, 1f, 0.35f);

    private struct BarkData
    {
        public string message;
        public float duration;
        public bool isPlayer;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        CreateBarkUI();
        barkContainer.SetActive(false);
    }

    private void CreateBarkUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Container principal
        barkContainer = new GameObject("BarkContainer");
        barkContainer.transform.SetParent(canvas.transform, false);

        RectTransform containerRT = barkContainer.AddComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(1f, 0f);
        containerRT.anchorMax = new Vector2(1f, 0f);
        containerRT.pivot = new Vector2(1f, 0f);
        containerRT.anchoredPosition = new Vector2(-20f, 20f);

        // Fond de la bulle
        barkBackground = barkContainer.AddComponent<Image>();
        barkBackground.color = commanderBgColor;

        // ContentSizeFitter — la bulle s'adapte au contenu
        ContentSizeFitter fitter = barkContainer.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // HorizontalLayoutGroup — padding interne
        HorizontalLayoutGroup layout = barkContainer.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(15, 15, 8, 8);
        layout.childAlignment = TextAnchor.MiddleRight;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        // Texte
        GameObject textObj = new GameObject("BarkText");
        textObj.transform.SetParent(barkContainer.transform, false);

        barkText = textObj.AddComponent<Text>();
        barkText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        barkText.fontSize = 22;
        barkText.alignment = TextAnchor.MiddleRight;
        barkText.color = commanderColor;
        barkText.horizontalOverflow = HorizontalWrapMode.Wrap;
        barkText.verticalOverflow = VerticalWrapMode.Overflow;

        // LayoutElement — largeur max pour forcer le retour à la ligne
        LayoutElement textLayout = textObj.AddComponent<LayoutElement>();
        textLayout.preferredWidth = 400f;
    }

    /// <summary>
    /// Affiche un bark. Mis en queue si un bark est déjà en cours.
    /// </summary>
    /// <param name="message">Le texte à afficher.</param>
    /// <param name="duration">Durée d'affichage en secondes.</param>
    /// <param name="isPlayer">True = PILOTE (cyan), False = COMMANDANT (blanc).</param>
    public void ShowBark(string message, float duration = 3f, bool isPlayer = false)
    {
        BarkData data = new BarkData
        {
            message = message,
            duration = duration,
            isPlayer = isPlayer
        };

        barkQueue.Enqueue(data);

        if (!isShowingBark)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowingBark = true;

        while (barkQueue.Count > 0)
        {
            BarkData data = barkQueue.Dequeue();

            // Appliquer les couleurs selon le locuteur
            if (data.isPlayer)
            {
                barkText.color = playerColor;
                barkBackground.color = playerBgColor;
                barkText.text = "[PILOTE] " + data.message;
            }
            else
            {
                barkText.color = commanderColor;
                barkBackground.color = commanderBgColor;
                barkText.text = "[COMMANDANT] " + data.message;
            }

            barkContainer.SetActive(true);
            yield return new WaitForSeconds(data.duration);
            barkContainer.SetActive(false);
            yield return new WaitForSeconds(0.3f); // Pause entre deux bulles
        }

        isShowingBark = false;
    }
}
