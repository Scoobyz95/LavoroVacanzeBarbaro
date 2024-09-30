using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab; // Il prefab della macchina
    public Transform spawnPoint; // Il punto di spawn
    public Button spawnButton; // Il bottone per generare la macchina

    void Start()
    {
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(SpawnCar);
        }
        else
        {
            Debug.LogError("Bottone non assegnato!");
        }
    }

    void SpawnCar()
    {
        if (carPrefab != null && spawnPoint != null)
        {
            Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogError("Prefab della macchina o punto di spawn non assegnato!");
        }
    }
}
