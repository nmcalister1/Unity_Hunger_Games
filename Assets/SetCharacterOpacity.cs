using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;


    public class SetCharacterOpacity : MonoBehaviour
    {
        //public GameObject kaykitAnimatedCharacter; // Reference to the kaykit animated character
        public TMP_Text invisibilityMessage;
        public TMP_Text visibleMessage;

        // private class MaterialState
        // {
        //     public float OriginalAlpha;
        //     public int OriginalDstBlend;
        //     public bool IsAlphaBlendEnabled;
        // }

        // private Dictionary<Material, MaterialState> originalMaterialStates = new Dictionary<Material, MaterialState>();

        // [PunRPC]
        // public void SetOpacity()
        // {
        //     Debug.Log("Setting opacity to 50%");
        //     // Get references to the parts with Mesh Renderers
        //     Transform body = kaykitAnimatedCharacter.transform.Find("Body");
        //     Transform armLeft = body.Find("armLeft/character_barbarianArmLeft");
        //     Transform armRight = body.Find("armRight/character_barbarianArmRight");
        //     Transform head = body.Find("Head/character_barbarianHead");
        //     Transform hat = body.Find("Head/character_barbarianHead/character_barbarianHat");
        //     Transform bruno = body.Find("Bruno");

        //     // Apply opacity to each part
        //     ApplyOpacity(armLeft.GetComponent<MeshRenderer>());
        //     ApplyOpacity(armRight.GetComponent<MeshRenderer>());
        //     ApplyOpacity(head.GetComponent<MeshRenderer>());
        //     ApplyOpacity(hat.GetComponent<MeshRenderer>());
        //     ApplyOpacity(bruno.GetComponent<MeshRenderer>());

        //     // Show the invisibility message
        //     ShowInvisibilityMessage();
        // }

        // [PunRPC]
        // public void ResetOpacity()
        // {
        //     // Get references to the parts with Mesh Renderers
        //     Transform body = kaykitAnimatedCharacter.transform.Find("Body");
        //     Transform armLeft = body.Find("armLeft/character_barbarianArmLeft");
        //     Transform armRight = body.Find("armRight/character_barbarianArmRight");
        //     Transform head = body.Find("Head/character_barbarianHead");
        //     Transform hat = body.Find("Head/character_barbarianHead/character_barbarianHat");
        //     Transform bruno = body.Find("Bruno");

        //     // Reset material properties for each part
        //     ResetMaterialProperties(armLeft.GetComponent<MeshRenderer>());
        //     ResetMaterialProperties(armRight.GetComponent<MeshRenderer>());
        //     ResetMaterialProperties(head.GetComponent<MeshRenderer>());
        //     ResetMaterialProperties(hat.GetComponent<MeshRenderer>());
        //     ResetMaterialProperties(bruno.GetComponent<MeshRenderer>());
        // }

        // private void ApplyOpacity(MeshRenderer meshRenderer)
        // {
        //     if (meshRenderer != null)
        //     {
        //         foreach (Material mat in meshRenderer.materials)
        //         {
        //             if (!originalMaterialStates.ContainsKey(mat))
        //             {
        //                 // Store the original material state
        //                 originalMaterialStates[mat] = new MaterialState
        //                 {
        //                     OriginalAlpha = mat.color.a,
        //                     OriginalDstBlend = mat.GetInt("_DstBlend"),
        //                     IsAlphaBlendEnabled = mat.IsKeywordEnabled("_ALPHABLEND_ON")
        //                 };
        //             }
        //             // Enable transparency
        //             //mat.SetOverrideTag("RenderType", "Transparent");
        //             //mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //             mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //             //mat.SetInt("_ZWrite", 0);
        //             //mat.DisableKeyword("_ALPHATEST_ON");
        //             mat.EnableKeyword("_ALPHABLEND_ON");
        //             //mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //             //mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        //             // Set the alpha value to 0.5 (50% opacity)
        //             Color color = mat.color;
        //             color.a = 0.5f;
        //             mat.color = color;

        //             Debug.Log($"Applied opacity to material: {mat.name}, new alpha: {color.a}");
        //         }
        //     }
        // }

        // private void ResetMaterialProperties(MeshRenderer meshRenderer)
        // {
        //     if (meshRenderer != null)
        //     {
        //         foreach (Material mat in meshRenderer.materials)
        //         {
        //             if (originalMaterialStates.ContainsKey(mat))
        //             {
        //                 // Restore the original material state
        //                 MaterialState state = originalMaterialStates[mat];

        //                 Color color = mat.color;
        //                 color.a = state.OriginalAlpha;
        //                 mat.color = color;

        //                 mat.SetInt("_DstBlend", state.OriginalDstBlend);

        //                 if (state.IsAlphaBlendEnabled)
        //                 {
        //                     mat.EnableKeyword("_ALPHABLEND_ON");
        //                 }
        //                 else
        //                 {
        //                     mat.DisableKeyword("_ALPHABLEND_ON");
        //                 }
        //             }
        //         }
        //     }
        // }

        public void ShowInvisibilityMessage()
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

        public void ShowVisibleMessage()
        {
            if (visibleMessage != null)
            {
                visibleMessage.gameObject.SetActive(true); // Show the message
                Invoke("HideVisibleMessage", 2f); // Hide the message after 2 seconds
            }
        }

        private void HideVisibleMessage()
        {
            if (visibleMessage != null)
            {
                visibleMessage.gameObject.SetActive(false); // Hide the message
            }
        }

    }


