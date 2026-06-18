using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;
    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.position;
    }

    /// <summary>
    /// Triggers a camera shake effect for the given duration and intensity.
    /// </summary>
    public void Shake(float duration = 0.2f, float intensity = 0.15f)
    {
        StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-intensity, intensity);
            float y = originalPos.y + Random.Range(-intensity, intensity);
            transform.position = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;
    }
}
