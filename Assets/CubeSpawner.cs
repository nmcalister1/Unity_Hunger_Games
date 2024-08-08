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
    public GameObject swordsPrefab;
    public float spawnInterval = 15f; // Time interval in seconds

    private List<Vector3> spawnTilePositions = new List<Vector3>(); // List to store spawn tile positions
    private GameObject[] prefabs; // Array to store all prefabs
    private float[] probabilities; // Array to store probabilities
    //private float chickenProbability = 0.16f; // Initial probability of chickenPrefab

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Collect all positions of objects tagged as 'SpawnTile'
            CollectSpawnTilePositions();

            // Initialize the array with all the prefabs
            prefabs = new GameObject[]
            {
                fastFeetPrefab,
                lightningPrefab,
                laserGunPrefab,
                rocketLauncherPrefab,
                invisibilityCloakPrefab,
                chickenPrefab,
                minesPrefab,
                swordsPrefab
            };

            // Initialize the probabilities
            probabilities = new float[]
            {
                0.125f, // fastFeetPrefab
                0.125f, // lightningPrefab
                0.125f, // laserGunPrefab
                0.125f, // rocketLauncherPrefab
                0.125f, // invisibilityCloakPrefab
                0.125f, // chickenPrefab
                0.125f, // minesPrefab
                0.125f  // swordsPrefab
            };

            // Spawn the initial set of prefabs
            for (int i = 0; i < 10; i++)
            {
                SpawnCube();
            }

            // Start the routine to spawn more prefabs at intervals
            StartCoroutine(SpawnCubeRoutine());
        }
    }

    private void CollectSpawnTilePositions()
    {
        // Find all GameObjects with the tag 'SpawnTile'
        GameObject[] spawnTiles = GameObject.FindGameObjectsWithTag("SpawnTile");

        foreach (GameObject spawnTile in spawnTiles)
        {
            Vector3 position = spawnTile.transform.position;
            spawnTilePositions.Add(position);
        }

        Debug.Log($"Collected {spawnTilePositions.Count} spawn tile positions.");
    }

    private IEnumerator SpawnCubeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Spawn the specified number of prefabs at each interval
            for (int i = 0; i < 7; i++)
            {
                SpawnCube();
            }
        }
    }

    // private IEnumerator DecreaseChickenProbabilityRoutine()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(15f);

    //         // Decrease the chicken probability by 0.01%
    //         chickenProbability -= 0.0001f;

    //         // Ensure it doesn't go below 0
    //         chickenProbability = Mathf.Max(chickenProbability, 0f);

    //         // Update the probabilities array
    //         probabilities[5] = chickenProbability;

    //         Debug.Log($"Updated chicken probability: {chickenProbability}");
    //     }
    // }

    private void SpawnCube()
    {
        if (spawnTilePositions.Count == 0)
        {
            Debug.Log("No spawn tiles available for spawning.");
            return;
        }

        // Choose a random position
        Vector3 randomPosition = spawnTilePositions[Random.Range(0, spawnTilePositions.Count)];

        // Choose a random prefab based on probabilities
        GameObject prefabToSpawn = ChoosePrefab();

        // Offset the spawn position to sit on top of the tile, accounting for cube height
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


