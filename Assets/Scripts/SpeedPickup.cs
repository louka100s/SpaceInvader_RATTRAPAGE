using UnityEngine;

public class SpeedPickup : MonoBehaviour
{
    public float fallSpeed = 3f;

    private const float DestroyBelowY = -7f;
    private const float RotationSpeed = 180f;

    private void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);

        if (transform.position.y < DestroyBelowY)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.ActivateSpeedBoost();

            Destroy(gameObject);
        }
    }
}
