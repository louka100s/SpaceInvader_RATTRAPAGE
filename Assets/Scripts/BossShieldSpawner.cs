using UnityEngine;

public class BossShieldSpawner : MonoBehaviour
{
    public int shieldCount = 3;
    public int shieldWidth = 3;
    public int shieldHeight = 2;
    public float blockSize = 0.4f;
    public float blockSpacing = 0.05f;
    public Material shieldMaterial;

    private void Start()
    {
        float totalSpread = 10f;
        float startX = -totalSpread / 2f;
        float spacing = totalSpread / (shieldCount - 1);

        // Boucliers du Boss en haut (Y = 2.5) — fixes, ne suivent pas le Boss
        for (int s = 0; s < shieldCount; s++)
        {
            float shieldX = startX + s * spacing;
            SpawnShieldGroup("BossShield_" + s, shieldX, 2.5f);
        }

        // Boucliers du joueur en bas (Y = -4)
        for (int s = 0; s < shieldCount; s++)
        {
            float shieldX = startX + s * spacing;
            SpawnShieldGroup("PlayerShield_" + s, shieldX, -4f);
        }
    }

    /// <summary>
    /// Spawns a shieldWidth x shieldHeight grid of destructible shield blocks at the given world position.
    /// Each block has a trigger BoxCollider, kinematic Rigidbody, and a Shield component.
    /// </summary>
    private void SpawnShieldGroup(string groupName, float worldX, float worldY)
    {
        GameObject shieldParent = new GameObject(groupName);
        shieldParent.transform.position = new Vector3(worldX, worldY, 0f);

        float step = blockSize + blockSpacing;

        for (int row = 0; row < shieldHeight; row++)
        {
            for (int col = 0; col < shieldWidth; col++)
            {
                GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = groupName.StartsWith("Boss") ? "BossShieldBlock" : "PlayerShieldBlock";
                block.transform.SetParent(shieldParent.transform);

                float xPos = (col - (shieldWidth - 1) / 2f) * step;
                float yPos = row * step;
                block.transform.localPosition = new Vector3(xPos, yPos, 0f);
                block.transform.localScale = new Vector3(blockSize, blockSize, blockSize);

                BoxCollider col3D = block.GetComponent<BoxCollider>();
                col3D.isTrigger = true;

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
