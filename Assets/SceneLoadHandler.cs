using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoadHandler : MonoBehaviour
{
    private void Awake()
    {
        // Ensure this GameObject persists across scene loads
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene is level 1
        if (scene.buildIndex == 1)
        {
            // Start a coroutine to wait and then destroy the Canvas
            StartCoroutine(WaitAndDestroyCanvas(5f)); // Wait for 7 seconds before destroying
        }
    }

    private IEnumerator WaitAndDestroyCanvas(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Find the GameObject with the tag "ConnectToGame"
        GameObject gameObjectToDestroy = GameObject.FindGameObjectWithTag("ConnectToGame");
        if (gameObjectToDestroy != null)
        {
            Destroy(gameObjectToDestroy);
            Debug.Log("GameObject with tag 'ConnectToGame' destroyed after delay.");
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'ConnectToGame' found.");
        }
    }
}
