using UnityEngine;

public class ZoneInterdite : MonoBehaviour
{
    [SerializeField] private Transform pointDepart;

    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (!autre.CompareTag(Tags.Player))
        {
            return;
        }

        if (pointDepart == null)
        {
            Debug.LogError("Le point de départ n'est pas assigné.");
            return;
        }

        autre.transform.position = pointDepart.position;
        Debug.Log("Le robot retourne au point de départ.");
    }
}
