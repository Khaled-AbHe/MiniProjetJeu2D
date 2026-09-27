using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damageValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Tags.Player))
        {
            return;
        }

        if (collision.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damageValue);
        }
    }
}
