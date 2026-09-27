using UnityEngine;

public class Collecteur : MonoBehaviour
{
    [SerializeField] private int objectif = 3;
    [SerializeField] private GameObject porteSortie;

    private int batteriesCollectees = 0;

    private void Start()
    {
        if (porteSortie == null)
        {
            Debug.LogError("La porte de sortie n'est pas assignée.");
            return;
        }

        porteSortie.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (!autre.CompareTag(Tags.Batterie))
        {
            return;
        }

        batteriesCollectees++;
        Debug.Log($"Batteries : {batteriesCollectees}/{objectif}");

        Destroy(autre.gameObject);

        if (batteriesCollectees >= objectif)
        {
            porteSortie.SetActive(true);
            Debug.Log("PORTE DÉVERROUILLÉE !");
        }
    }
}
