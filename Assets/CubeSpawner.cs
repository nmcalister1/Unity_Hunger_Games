using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun; // Import Photon.Pun

public class CubeSpawner : MonoBehaviourPunCallbacks
{
    public GameObject fastFeetPrefab;
    public GameObject lightningPrefab;
    public GameObject laserGunPrefab;
    public GameObject rocketLauncherPrefab;
    public GameObject invisibilityCloakPrefab;
    public GameObject chickenPrefab;
    public GameObject minesPrefab;
    public float spawnInterval = 15f; // Time interval in seconds
    public Grid terrainGrid; // Reference to the terrain grid

    private List<Vector3> tilePositions = new List<Vector3>(); // List to store tile positions
    private GameObject[] prefabs; // Array to store all prefabs
    private float[] probabilities; // Array to store probabilities
    private float chickenProbability = 0.16f; // Initial probability of chickenPrefab

    private void Start()
    {
        // Collect all tile positions from all tilemaps in the terrain grid
        CollectGameObjectPositions();

        // Initialize the array with all the prefabs
        prefabs = new GameObject[]
        {
            fastFeetPrefab,
            lightningPrefab,
            laserGunPrefab,
            rocketLauncherPrefab,
            invisibilityCloakPrefab,
            chickenPrefab,
            minesPrefab
        };

        // Initialize the probabilities
        probabilities = new float[]
        {
            0.12f, // fastFeetPrefab
            0.12f, // lightningPrefab
            0.12f, // laserGunPrefab
            0.12f, // rocketLauncherPrefab
            0.12f, // invisibilityCloakPrefab
            chickenProbability, // chickenPrefab
            0.12f  // minesPrefab
        };

        // Spawn the initial set of prefabs
        for (int i = 0; i < 10; i++)
        {
            SpawnCube();
        }

        // Start the routine to spawn more prefabs at intervals
        StartCoroutine(SpawnCubeRoutine());

        // Start the routine to decrease the chicken probability
        StartCoroutine(DecreaseChickenProbabilityRoutine());
    }

    private void CollectGameObjectPositions()
    {
        // Get all tilemaps in the terrain grid
        Tilemap[] tilemaps = terrainGrid.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (Transform child in tilemap.transform)
            {
                if (child.gameObject != null)
                {
                    Vector3 position = child.position;
                    tilePositions.Add(position);
                }
            }
        }

        Debug.Log($"Collected {tilePositions.Count} tile positions.");
    }

    private IEnumerator SpawnCubeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Spawn the specified number of prefabs at each interval
            for (int i = 0; i < 10; i++)
            {
                SpawnCube();
            }
        }
    }

    private IEnumerator DecreaseChickenProbabilityRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);

            // Decrease the chicken probability by 0.01%
            chickenProbability -= 0.0001f;

            // Ensure it doesn't go below 0
            chickenProbability = Mathf.Max(chickenProbability, 0f);

            // Update the probabilities array
            probabilities[5] = chickenProbability;

            Debug.Log($"Updated chicken probability: {chickenProbability}");
        }
    }

    private void SpawnCube()
    {
        if (tilePositions.Count == 0)
        {
            Debug.Log("No tiles available for spawning.");
            return;
        }

        // Choose a random position
        Vector3 randomPosition = tilePositions[Random.Range(0, tilePositions.Count)];

        // Choose a random prefab based on probabilities
        GameObject prefabToSpawn = ChoosePrefab();

        // Offset the spawn position to sit on top of the GameObject, accounting for cube height
        Vector3 spawnPosition = randomPosition + new Vector3(0, 1.5f, 0);

        // Use PhotonNetwork.Instantiate to spawn the prefab
        if (prefabToSpawn != null)
        {
            string prefabName = prefabToSpawn.name; // Make sure this matches the prefab name in the Resources folder
            PhotonNetwork.Instantiate("Prefabs/" + prefabName, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab to spawn is null.");
        }
    }

    private GameObject ChoosePrefab()
    {
        float total = 0f;
        foreach (float prob in probabilities)
        {
            total += prob;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probabilities.Length; i++)
        {
            if (randomPoint < probabilities[i])
            {
                return prefabs[i];
            }
            else
            {
                randomPoint -= probabilities[i];
            }
        }

        return null; // Fallback in case something goes wrong
    }
}


