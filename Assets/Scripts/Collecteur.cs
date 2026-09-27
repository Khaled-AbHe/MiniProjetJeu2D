using System;
using UnityEngine;

public class Collecteur : MonoBehaviour
{
    [SerializeField] private int objectif = 3;
    [SerializeField] private GameObject porteSortie;

    [Tooltip("Physical barrier blocking the path to the exit. Destroyed once the objective is met, opening the way through.")]
    [SerializeField] private GameObject gate;

    private int batteriesCollectees = 0;

    /// <summary>
    /// Fired whenever the collected count changes, and once at Start with
    /// the initial 0/objectif. BatteryCounterUI (and anything else) can
    /// subscribe to this to display progress. (currentCount, objectif)
    /// </summary>
    public static event Action<int, int> OnBatteryCountChanged;

    private void Start()
    {
        if (porteSortie == null)
        {
            Debug.LogError("La porte de sortie n'est pas assignée.");
        }
        else
        {
            porteSortie.SetActive(false);
        }

        OnBatteryCountChanged?.Invoke(batteriesCollectees, objectif);
    }

    private void OnTriggerEnter2D(Collider2D autre)
    {
        if (!autre.CompareTag(Tags.Batterie))
        {
            return;
        }

        batteriesCollectees++;
        Debug.Log($"Batteries : {batteriesCollectees}/{objectif}");
        OnBatteryCountChanged?.Invoke(batteriesCollectees, objectif);

        Destroy(autre.gameObject);

        if (batteriesCollectees >= objectif)
        {
            if (porteSortie != null) porteSortie.SetActive(true);
            Debug.Log("PORTE DÉVERROUILLÉE !");

            if (gate != null)
            {
                Destroy(gate);
            }
        }
    }
}
