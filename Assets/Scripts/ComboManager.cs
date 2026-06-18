using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    public float comboResetTime = 2.5f;

    private int combo = 0;
    private float comboTimer = 0f;
    private Text comboText;
    private RectTransform comboTextRT;
    private Vector3 targetScale = Vector3.one;

    private const float PunchScale = 1.3f;
    private const float ScaleLerpSpeed = 10f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateComboUI();
        comboText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Décrémenter le timer et reset le combo si expiré
        if (combo > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                combo = 0;
                comboText.gameObject.SetActive(false);
            }
        }

        // Lerp du scale punch vers la normale
        if (comboTextRT != null)
        {
            comboTextRT.localScale = Vector3.Lerp(comboTextRT.localScale, targetScale, Time.deltaTime * ScaleLerpSpeed);
        }
    }

    /// <summary>
    /// Called whenever the player destroys an enemy or UFO.
    /// Awards bonus score when a combo streak of 2 or more is reached.
    /// </summary>
    public void RegisterKill()
    {
        combo++;
        comboTimer = comboResetTime;

        if (combo >= 2)
        {
            // Score bonus : combo - 1 points additionnels
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(combo - 1);
            }

            // Afficher et mettre à jour le texte
            comboText.gameObject.SetActive(true);
            comboText.text = "x" + combo;

            // Scale punch
            comboTextRT.localScale = Vector3.one * PunchScale;
        }
    }

    private void CreateComboUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject textObj = new GameObject("ComboText");
        textObj.transform.SetParent(canvas.transform, false);

        comboTextRT = textObj.AddComponent<RectTransform>();
        comboTextRT.anchorMin = new Vector2(0f, 0f);
        comboTextRT.anchorMax = new Vector2(0f, 0f);
        comboTextRT.pivot = new Vector2(0f, 0f);
        comboTextRT.anchoredPosition = new Vector2(20f, 20f);
        comboTextRT.sizeDelta = new Vector2(200f, 60f);

        comboText = textObj.AddComponent<Text>();
        comboText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        comboText.fontSize = 40;
        comboText.fontStyle = FontStyle.Bold;
        comboText.alignment = TextAnchor.MiddleLeft;
        comboText.color = Color.yellow;

        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
    }
}
