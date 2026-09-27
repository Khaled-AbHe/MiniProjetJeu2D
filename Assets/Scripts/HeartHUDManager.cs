using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartHUDManager : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Setup")]
    [SerializeField] private GameObject heartPrefab; // A UI Image prefab

    private List<Image> spawnedHearts = new List<Image>();

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHeartsHUD;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHeartsHUD;
    }

    private void UpdateHeartsHUD(int currentHealth, int maxHealth)
    {
        // 1. Each 1 point of maxHealth now equals exactly 1 heart container
        int totalHeartsNeeded = maxHealth;

        // 2. Adjust spawned UI icons if max health changed
        AdjustHeartContainers(totalHeartsNeeded);

        // 3. Loop through each heart container and set it to full or empty
        for (int i = 0; i < spawnedHearts.Count; i++)
        {
            // If the current health is greater than the index of this heart, it's full
            if (currentHealth > i)
            {
                spawnedHearts[i].sprite = fullHeart;
            }
            else
            {
                spawnedHearts[i].sprite = emptyHeart;
            }
        }
    }

    private void AdjustHeartContainers(int totalHeartsNeeded)
    {
        while (spawnedHearts.Count < totalHeartsNeeded)
        {
            if (heartPrefab == null) return;

            GameObject newHeart = Instantiate(heartPrefab, transform);
            Image heartImage = newHeart.GetComponent<Image>();

            if (heartImage != null)
            {
                spawnedHearts.Add(heartImage);
            }
        }

        while (spawnedHearts.Count > totalHeartsNeeded)
        {
            Image heartToRemove = spawnedHearts[spawnedHearts.Count - 1];
            spawnedHearts.Remove(heartToRemove);
            if (heartToRemove != null) Destroy(heartToRemove.gameObject);
        }
    }
}
