using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    public int shieldCount = 3;
    public int shieldWidth = 3;
    public int shieldHeight = 2;
    public float shieldY = -3.5f;
    public float blockSize = 0.4f;
    public float blockSpacing = 0.05f;
    public Material shieldMaterial;

    /// <summary>
    /// Spawns shieldCount destructible shields between the player and the enemy grid.
    /// Each shield is made of shieldWidth x shieldHeight individual cube blocks,
    /// each with a trigger BoxCollider, kinematic Rigidbody, and Shield component.
    /// </summary>
    private void Start()
    {
        const float totalSpread = 12f;
        float startX = -totalSpread / 2f;
        float spacing = totalSpread / (shieldCount - 1);

        for (int s = 0; s < shieldCount; s++)
        {
            float shieldX = startX + s * spacing;

            GameObject shieldParent = new GameObject("Shield_" + s);
            shieldParent.transform.position = new Vector3(shieldX, shieldY, 0f);

            for (int row = 0; row < shieldHeight; row++)
            {
                for (int col = 0; col < shieldWidth; col++)
                {
                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    block.name = "ShieldBlock";
                    block.transform.SetParent(shieldParent.transform);

                    float step = blockSize + blockSpacing;
                    float xPos = (col - (shieldWidth - 1) / 2f) * step;
                    float yPos = row * step;
                    block.transform.localPosition = new Vector3(xPos, yPos, 0f);
                    block.transform.localScale = new Vector3(blockSize, blockSize, blockSize);

                    BoxCollider boxCollider = block.GetComponent<BoxCollider>();
                    boxCollider.isTrigger = true;

                    Rigidbody rb = block.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.isKinematic = true;

                    block.AddComponent<Shield>();

                    if (shieldMaterial != null)
                    {
                        block.GetComponent<Renderer>().material = shieldMaterial;
                    }
                }
            }
        }
    }
}
