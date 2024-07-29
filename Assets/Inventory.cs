using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;


namespace RPG.Character
{
   public class Inventory : MonoBehaviourPunCallbacks
   {
       public string currentInventory;  // Reference to the current inventory item
       public string currentWeapon; // Reference to the current weapon


       // Add references to the UI Image components
       public Image inventoryImage;


       // Dictionary to hold the sprites
       private Dictionary<string, Sprite> inventorySprites;


       void Start()
       {
           // Initialize the dictionary and load the sprites
           inventorySprites = new Dictionary<string, Sprite>();
           LoadInventorySprites();
       }


       private void LoadInventorySprites()
       {
           // Load all sprites from the Assets/Images folder
           Sprite[] sprites = Resources.LoadAll<Sprite>("Images");


           foreach (Sprite sprite in sprites)
           {
               // Add each sprite to the dictionary with the sprite name as the key
               inventorySprites[sprite.name] = sprite;
           }
       }


       public void ResetInventory()
       {
           currentInventory = null;
           inventoryImage.sprite = null;
       }


       public void PickUpItem(string itemName)
       {
           // Determine if the item is a weapon or inventory item
           if (IsWeapon(itemName))
           {
               if (currentWeapon == "RocketLauncher" && itemName == "LaserGun")
               {
                   photonView.RPC("RPC_DisableRocketLauncher", RpcTarget.AllBuffered);
               }
               if (currentWeapon == "LaserGun" && itemName == "RocketLauncher")
               {
                   photonView.RPC("RPC_DisableLaserGun", RpcTarget.AllBuffered);
               }
              
               currentWeapon = itemName;
               // Handle any additional weapon-related logic here
           }
           else
           {
               currentInventory = itemName;
               UpdateInventoryImage(itemName); // Update the inventory image with the corresponding sprite name
           }
       }


       private bool IsWeapon(string itemName)
       {
           // List of item names that are considered weapons
           string[] weapons = { "LaserGun", "RocketLauncher", "Mines", "Weapon" };


           foreach (string weapon in weapons)
           {
               if (itemName == weapon)
               {
                   return true;
               }
           }
           return false;
       }


       private void UpdateInventoryImage(string itemName)
       {
           Debug.Log($"Picked up {itemName}!");
           Debug.Log($"Current inventory: {currentInventory}");
           if (inventorySprites.TryGetValue(itemName, out Sprite sprite))
           {
               inventoryImage.sprite = sprite;
           }
           else
           {
               Debug.LogWarning($"Sprite for item '{itemName}' not found.");
           }
       }
   }
}

