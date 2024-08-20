using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine.Events;
using RPG.Character;


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
    public GameObject revealLocationPrefab;
    public float spawnInterval = 15f; // Time interval in seconds
    public UnityEvent onNewSupplies;
    private const byte NEW_SUPPLIES_EVENT = 1; // Event code for new supplies

    private List<Vector3> spawnTilePositions = new List<Vector3>(); // List to store spawn tile positions
    private GameObject[] prefabs; // Array to store all prefabs
    //private float[] probabilities; // Array to store probabilities
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
                swordsPrefab,
                revealLocationPrefab
            };

            // Initialize the probabilities
            // probabilities = new float[]
            // {
            //     0.125f, // fastFeetPrefab
            //     0.125f, // lightningPrefab
            //     0.125f, // laserGunPrefab
            //     0.125f, // rocketLauncherPrefab
            //     0.125f, // invisibilityCloakPrefab
            //     0.125f, // chickenPrefab
            //     0.125f, // minesPrefab
            //     0.125f  // swordsPrefab
            // };

            // Spawn the initial set of prefabs
            for (int i = 0; i < 10; i++)
            {
                SpawnCube();
            }

            // Start the routine to spawn more prefabs at intervals
            StartCoroutine(SpawnCubeRoutine());

            // Spawn items at the start of the game on specified tiles
            SpawnItemsAtStart();
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

            // Raise the UnityEvent
            PlayerController.OnNewSuppliesEvent.Invoke();

            // Spawn the specified number of prefabs at each interval
            for (int i = 0; i < 7; i++)
            {
                SpawnCube();
            }
        }
    }


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

        //Debug.Log($"Spawning {prefabToSpawn.name} at {spawnPosition}");

        // Use PhotonNetwork.Instantiate to spawn the prefab
        if (prefabToSpawn != null)
        {
            string prefabName = prefabToSpawn.name; // Make sure this matches the prefab name in the Resources folder
            Debug.Log($"Spawning {prefabName} at {spawnPosition}");
            PhotonNetwork.Instantiate("Prefabs/" + prefabName, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab to spawn is null.");
        }
    }

    private GameObject ChoosePrefab()
    {
        int randomIndex = Random.Range(0, prefabs.Length);
        return prefabs[randomIndex];
    }

    private void SpawnItemsAtStart()
    {
        // Find all GameObjects with the tag 'SpawnItemAtStart'
        GameObject[] startSpawnTiles = GameObject.FindGameObjectsWithTag("SpawnItemAtStart");

        if (startSpawnTiles.Length < 4)
        {
            Debug.LogWarning("Not enough spawn tiles with the tag 'SpawnItemAtStart'.");
            return;
        }

        // Ensure startSpawnTiles is ordered consistently, if necessary
        System.Array.Sort(startSpawnTiles, (tile1, tile2) => tile1.name.CompareTo(tile2.name));

        // Specify the prefabs to be spawned in the desired order
        GameObject[] itemsToSpawn = new GameObject[]
        {
            rocketLauncherPrefab,
            fastFeetPrefab,
            fastFeetPrefab,
            invisibilityCloakPrefab
        };

        // Iterate through the specified tiles and spawn the corresponding items
        for (int i = 0; i < itemsToSpawn.Length; i++)
        {
            Vector3 spawnPosition = startSpawnTiles[i].transform.position + new Vector3(0, 1.5f, 0);
            GameObject prefabToSpawn = itemsToSpawn[i];

            if (prefabToSpawn != null)
            {
                string prefabName = prefabToSpawn.name;
                PhotonNetwork.Instantiate("Prefabs/" + prefabName, spawnPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Prefab to spawn is null.");
            }
        }
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Master client switched to " + newMasterClient.NickName + ". Inside of cubespawner script");
        if (PhotonNetwork.IsMasterClient)
        {
            Start();
        }
    }
}


