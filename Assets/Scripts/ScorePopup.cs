using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    /// <summary>
    /// Spawns a "+N" text on the Canvas that floats upward and fades out.
    /// </summary>
    public static void Spawn(Vector3 worldPos, int points)
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject popup = new GameObject("ScorePopup");
        popup.transform.SetParent(canvas.transform, false);

        Text text = popup.AddComponent<Text>();
        text.text = "+" + points;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 30;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.yellow;

        RectTransform rt = popup.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120f, 50f);

        if (Camera.main != null)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            rt.position = screenPos;
        }

        popup.AddComponent<ScorePopup>();
    }

    private void Start() { StartCoroutine(PopupAnim()); }

    private IEnumerator PopupAnim()
    {
        Text text = GetComponent<Text>();
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 startPos = rt.position;
        float elapsed = 0f;

        while (elapsed < 0.8f)
        {
            rt.position = startPos + Vector3.up * elapsed * 80f;
            Color c = text.color;
            c.a = 1f - (elapsed / 0.8f);
            text.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
