using UnityEngine;
using TMPro;

public class SetCharacterOpacity : MonoBehaviour
{
    public GameObject kaykitAnimatedCharacter; // Reference to the kaykit animated character
    public TMP_Text invisibilityMessage;

    public void SetOpacity()
    {
        // Get references to the parts with Mesh Renderers
        Transform body = kaykitAnimatedCharacter.transform.Find("Body");
        Transform armLeft = body.Find("armLeft/character_barbarianArmLeft");
        Transform armRight = body.Find("armRight/character_barbarianArmRight");
        Transform head = body.Find("Head/character_barbarianHead");
        Transform hat = body.Find("Head/character_barbarianHead/character_barbarianHat");
        Transform bruno = body.Find("Bruno");

        // Apply opacity to each part
        ApplyOpacity(armLeft.GetComponent<MeshRenderer>());
        ApplyOpacity(armRight.GetComponent<MeshRenderer>());
        ApplyOpacity(head.GetComponent<MeshRenderer>());
        ApplyOpacity(hat.GetComponent<MeshRenderer>());
        ApplyOpacity(bruno.GetComponent<MeshRenderer>());

        // Show the invisibility message
        ShowInvisibilityMessage();
    }

    public void ResetOpacity()
    {
        // Get references to the parts with Mesh Renderers
        Transform body = kaykitAnimatedCharacter.transform.Find("Body");
        Transform armLeft = body.Find("armLeft/character_barbarianArmLeft");
        Transform armRight = body.Find("armRight/character_barbarianArmRight");
        Transform head = body.Find("Head/character_barbarianHead");
        Transform hat = body.Find("Head/character_barbarianHead/character_barbarianHat");
        Transform bruno = body.Find("Bruno");

        // Reset material properties for each part
        ResetMaterialProperties(armLeft.GetComponent<MeshRenderer>());
        ResetMaterialProperties(armRight.GetComponent<MeshRenderer>());
        ResetMaterialProperties(head.GetComponent<MeshRenderer>());
        ResetMaterialProperties(hat.GetComponent<MeshRenderer>());
        ResetMaterialProperties(bruno.GetComponent<MeshRenderer>());
    }

    private void ApplyOpacity(MeshRenderer meshRenderer)
    {
        if (meshRenderer != null)
        {
            foreach (Material mat in meshRenderer.materials)
            {
                mat.SetInt("_ZWrite", 0);
            }
        }
    }

    private void ResetMaterialProperties(MeshRenderer meshRenderer)
    {
        if (meshRenderer != null)
        {
            foreach (Material mat in meshRenderer.materials)
            {
                mat.SetInt("_ZWrite", 1); // Enable ZWrite
            }
        }
    }

    private void ShowInvisibilityMessage()
    {
        if (invisibilityMessage != null)
        {
            invisibilityMessage.gameObject.SetActive(true); // Show the message
            Invoke("HideInvisibilityMessage", 2f); // Hide the message after 2 seconds
        }
    }

    private void HideInvisibilityMessage()
    {
        if (invisibilityMessage != null)
        {
            invisibilityMessage.gameObject.SetActive(false); // Hide the message
        }
    }

}
