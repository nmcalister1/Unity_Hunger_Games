using UnityEngine;

public class TileColliderSetup : MonoBehaviour
{
    public PhysicMaterial noFrictionMaterial;
    public PhysicMaterial normalFrictionMaterial;

    void Start()
    {
        // Add top collider
        BoxCollider topCollider = gameObject.AddComponent<BoxCollider>();
        topCollider.size = new Vector3(1, 0.1f, 1); // Adjust height to cover only the top
        topCollider.center = new Vector3(0, .945f, 0); // Centered on top
        topCollider.material = normalFrictionMaterial;

        // Add side colliders
        AddSideCollider(new Vector3(.55f, 0, 0), new Vector3(0.1f, 1, 1)); // Right side
        AddSideCollider(new Vector3(-.55f, 0, 0), new Vector3(0.1f, 1, 1)); // Left side
        AddSideCollider(new Vector3(0, 0, .55f), new Vector3(1, 1, 0.1f)); // Front side
        AddSideCollider(new Vector3(0, 0, -.55f), new Vector3(1, 1, 0.1f)); // Back side
    }

    void AddSideCollider(Vector3 center, Vector3 size)
    {
        BoxCollider sideCollider = gameObject.AddComponent<BoxCollider>();
        sideCollider.size = size;
        sideCollider.center = center;
        sideCollider.material = noFrictionMaterial;
    }
}
