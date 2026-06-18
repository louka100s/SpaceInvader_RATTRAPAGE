using UnityEngine;
using System.Collections;

public class KillFlash : MonoBehaviour
{
    /// <summary>
    /// Spawns a small white cube that expands and fades out at the given world position.
    /// </summary>
    public static void Spawn(Vector3 position)
    {
        GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Cube);
        flash.name = "KillFlash";
        flash.transform.position = position;
        flash.transform.localScale = Vector3.one * 0.3f;
        Destroy(flash.GetComponent<BoxCollider>());
        flash.GetComponent<Renderer>().material.color = Color.white;
        flash.AddComponent<KillFlash>();
    }

    private void Start() { StartCoroutine(FlashAnim()); }

    private IEnumerator FlashAnim()
    {
        Renderer r = GetComponent<Renderer>();
        float elapsed = 0f;

        while (elapsed < 0.2f)
        {
            float scale = 0.3f + elapsed * 5f;
            transform.localScale = Vector3.one * scale;

            Color c = r.material.color;
            c.a = 1f - (elapsed / 0.2f);
            r.material.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
