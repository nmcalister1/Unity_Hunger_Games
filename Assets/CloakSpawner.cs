using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun; // Import Photon.Pun

public class CloakSpawner : MonoBehaviour
{
    public GameObject itemPrefab; // Assign the cloak prefab in the inspector

    private void Start()
    {
        SpawnInvisibilityCloak();
    }

    private void SpawnInvisibilityCloak()
    {
        // Find the game object with the 'SpawnCloak' tag
        GameObject spawnCloakCube = GameObject.FindWithTag("SpawnCloak");
        
        if (spawnCloakCube != null)
        {
            // Instantiate the cloak prefab at the position of the cube
            Vector3 spawnPosition = spawnCloakCube.transform.position;

            // Adjust the position to ensure the cloak is properly placed
            spawnPosition.y += 1.5f; // Adjust this value as needed

            // Instantiate the cloak
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No game object with the 'SpawnCloak' tag found.");
        }
    }
}
