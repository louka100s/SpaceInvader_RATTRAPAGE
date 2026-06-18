using UnityEngine;
using System.Collections;

public class UFOSpawner : MonoBehaviour
{
    public float minInterval = 15f;
    public float maxInterval = 30f;
    public float spawnY = 5f;
    public float spawnX = 10f;
    public Material ufoMaterial;

    private GameObject currentUFO;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Waits a fixed delay, then repeatedly spawns a UFO at random intervals.
    /// Only one UFO can be active at a time.
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            if (currentUFO != null) continue;

            SpawnUFO();
        }
    }

    /// <summary>
    /// Instantiates a UFO cube from a random side with all required components attached.
    /// Direction is chosen randomly: 1 = left to right, -1 = right to left.
    /// </summary>
    private void SpawnUFO()
    {
        int direction = Random.value > 0.5f ? 1 : -1;
        float startX = -direction * spawnX;

        GameObject ufo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ufo.name = "UFO";
        ufo.tag = "UFO";
        ufo.transform.position = new Vector3(startX, spawnY, 0f);
        ufo.transform.localScale = new Vector3(1.2f, 0.4f, 0.4f);

        BoxCollider col = ufo.GetComponent<BoxCollider>();
        col.isTrigger = true;

        Rigidbody rb = ufo.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        if (ufoMaterial != null)
        {
            ufo.GetComponent<Renderer>().material = ufoMaterial;
        }

        UFO ufoScript = ufo.AddComponent<UFO>();
        ufoScript.Launch(direction);

        currentUFO = ufo;
    }
}
