using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;




namespace RPG.Character
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject cameraHolder;
        [SerializeField] private float mouseSensitivity = 2f, walkSpeed = 4f, jumpForce = 5f, smoothTime = 0.1f, sprintSpeed = 6f;
        [SerializeField] private GameObject fastFeetEffectPrefab; // Reference to the Fast Feet effect prefab
        [SerializeField] private GameObject lightningEffectPrefab; // Reference to the Lightning effect prefab
        [SerializeField] private GameObject sciFiRifle;




        [SerializeField] private GameObject rocketLauncher;
        public GameObject crosshair;








        private float verticalLookRotation;
        private bool grounded;
        private Vector3 smothMoveVelocity;
        private Vector3 moveAmount;




        [SerializeField] private Camera playerCamera;




        private Rigidbody rb;
        private PhotonView PV;
        private Animator animator;
        private Health health; // Reference to the Health component
        private Inventory inventory; // Reference to the Inventory component




        private GameObject currentEffect; // To keep track of the current effect
        private Coroutine fastFeetCoroutine; // To keep track of the fast feet coroutine
        private Coroutine invisibilityCoroutine; // To keep track of the invisibility coroutine




        private float originalSpeed; // Store the original walking speed
        private bool isInvisible; // To keep track of the invisibility state




        public GameObject laserProjectilePrefab;
        public GameObject rocketProjectilePrefab;
        // public Transform projectileSpawnPoint;
        private int shotsRemaining = 3;




        private HashSet<Collider> processedColliders = new HashSet<Collider>();




        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            PV = GetComponent<PhotonView>();
            animator = GetComponentInChildren<Animator>();
            health = GetComponent<Health>(); // Get the Health component
            inventory = GetComponent<Inventory>(); // Get the Inventory component
            originalSpeed = walkSpeed; // Store the original walking speed

        }




        private void Start()
        {
            if (!PV.IsMine)
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(rb);
            }
        }




        private void Update()
        {
            if (!PV.IsMine) return;
            Look();
            Move();
            Jump();
            UseFastFeet();
            UseLightning();
            ConsumeChicken();
            UseInvisibilityCloak();
            //UseWeapon();
            ToggleCrosshair();
            ShootLaserGun();
            ShootRocketLauncher();
        }




        private void Look()
        {
            transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);




            verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);




            cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        }




        private void Move()
        {
            Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smothMoveVelocity, smoothTime);
        }




        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                rb.AddForce(transform.up * jumpForce);
            }
        }




        public void SetGroundedState(bool _grounded)
        {
            grounded = _grounded;
        }




        private void FixedUpdate()
        {
            if (!PV.IsMine) return;
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }




        // private void Attack()
        // {
        //     if (Input.GetMouseButtonDown(0)) // Left mouse button
        //     {
        //         Debug.Log("Player attacked!"); // Debug message for attack
        //         animator.SetTrigger("Attack"); // Trigger attack animation
        //         // Add attack logic here (e.g., detecting hit, dealing damage)
        //     }
        // }




        [PunRPC]
        public void TakeDamage(int damage)
        {
            if (PV.IsMine)
            {
                // Reduce the player's health
                health.currentHealth -= damage;
                if (health.currentHealth <= 0)
                {
                    // Handle player death
                    Die();
                }
            }
        }




        private void Die()
        {
            // You can add logic to handle what happens when the player dies
            Debug.Log("Player has died!");




            // For example, you can disable the player, show a respawn screen, etc.
            gameObject.SetActive(false);
        }












        private void OnTriggerEnter(Collider other)
        {
            if (processedColliders.Contains(other))
            {
                return; // Skip if this collider has already been processed
            }
            processedColliders.Add(other);




            if (other.CompareTag("FastFeet"))
            {
                PickUpItem("fastFeet", other.gameObject);
            }
            else if (other.CompareTag("Weapon"))
            {
                PickUpItem("Weapon", other.gameObject);
            }
            else if (other.CompareTag("Lightning"))
            {
                PickUpItem("lightning", other.gameObject);
            }
            else if (other.CompareTag("LaserGun"))
            {
                PickUpItem("LaserGun", other.gameObject);
            }
            else if (other.CompareTag("RocketLauncher"))
            {
                PickUpItem("RocketLauncher", other.gameObject);
            }
            else if (other.CompareTag("Cloak"))
            {
                PickUpItem("invisibilityCloak", other.gameObject);
            }
            else if (other.CompareTag("Chicken"))
            {
                PickUpItem("chicken", other.gameObject);
            }
            else if (other.CompareTag("Mines"))
            {
                PickUpItem("Mines", other.gameObject);
            }
        }




        private void PickUpItem(string itemName, GameObject itemObject)
        {
            if (!PV.IsMine) return;




            Debug.Log($"Picked up {itemName}!");
            inventory.PickUpItem(itemName); // Modify the Inventory script to handle this method
            photonView.RPC("RemoveItem", RpcTarget.AllBuffered, itemObject.GetComponent<PhotonView>().ViewID); // Remove item from all clients




            if (itemName == "LaserGun")
            {
                shotsRemaining = 3; // Set the number of shots remaining
                EnableLaserGun();
                photonView.RPC("RPC_EnableLaserGun", RpcTarget.AllBuffered);
            }




            if (itemName == "RocketLauncher")
            {
                shotsRemaining = 3; // Set the number of shots remaining
                EnableRocketLauncher();
                photonView.RPC("RPC_EnableRocketLauncher", RpcTarget.AllBuffered);

            }




            if (itemName == "Mines")
            {
                shotsRemaining = 3; // Set the number of shots remaining
                                    // EnableMines();
                                    // photonView.RPC("RPC_EnableMines", RpcTarget.AllBuffered);

            }




            if (itemName == "Weapon")
            {
                shotsRemaining = 3; // Set the number of shots remaining
                                    // EnableWeapon();
                                    // photonView.RPC("RPC_EnableWeapon", RpcTarget.AllBuffered);

            }
        }




        private void EnableLaserGun()
        {
            if (sciFiRifle != null)
            {
                sciFiRifle.SetActive(true);
            }
        }




        [PunRPC]
        private void RPC_EnableLaserGun()
        {
            EnableLaserGun();
        }




        private void EnableRocketLauncher()
        {
            if (rocketLauncher != null)
            {
                Debug.Log("Rocket Launcher activated!");
                rocketLauncher.SetActive(true);
            }
        }




        [PunRPC]
        private void RPC_EnableRocketLauncher()
        {
            EnableRocketLauncher();
        }




        [PunRPC]
        private void RemoveItem(int viewID)
        {
            PhotonView itemPV = PhotonView.Find(viewID);
            if (itemPV != null)
            {
                Destroy(itemPV.gameObject); // Remove item from the scene
            }
        }




        private void UseFastFeet()
        {
            if (inventory.currentInventory != null && inventory.currentInventory == "fastFeet" && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Fast feet activated!");
                inventory.ResetInventory(); // Reset the inventory




                // If there's an ongoing fast feet effect, stop it
                if (fastFeetCoroutine != null)
                {
                    StopCoroutine(fastFeetCoroutine);
                    walkSpeed = originalSpeed; // Ensure the speed is reset immediately
                }




                // Start a new fast feet effect
                PV.RPC("ActivateFastFeetEffect", RpcTarget.All);
            }
        }




        [PunRPC]
        private void ActivateFastFeetEffect()
        {
            if (currentEffect != null) Destroy(currentEffect); // Remove existing effect if any
            currentEffect = Instantiate(fastFeetEffectPrefab, transform.position, Quaternion.identity, transform); // Attach to player




            walkSpeed = 6f; // Increase speed to 6
            StartCoroutine(DeactivateFastFeetEffectAfterDelay(10f));
        }




        private IEnumerator DeactivateFastFeetEffectAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);




            // Reset speed to original value
            walkSpeed = originalSpeed;




            // Remove the effect after delay
            Destroy(currentEffect);
            fastFeetCoroutine = null; // Reset the coroutine reference
        }




        private void UseLightning()
        {
            if (inventory.currentInventory != null && inventory.currentInventory == "lightning" && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Lightning activated!");
                inventory.ResetInventory(); // Reset the inventory
                StartCoroutine(ActivateLightning());
            }
        }




        private IEnumerator ActivateLightning()
        {
            Debug.Log("Activating Lightning");




            // Detect all players within the 10-block radius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);
            foreach (var hitCollider in hitColliders)
            {
                PlayerController player = hitCollider.GetComponent<PlayerController>();
                if (player != null && player != this) // Skip the player who activated lightning
                {
                    PhotonView targetPV = player.GetComponent<PhotonView>();
                    if (targetPV != null)
                    {
                        Debug.Log($"Damaging player {targetPV.ViewID} at {targetPV.transform.position}");
                        targetPV.RPC("TakeDamage", RpcTarget.All, 10); // Apply damage
                        targetPV.RPC("ShowLightningEffect", RpcTarget.All, player.transform.position); // Show effect
                    }
                }
            }




            yield return null; // Optional, you can remove this if it's not necessary
        }




        [PunRPC]
        private void ShowLightningEffect(Vector3 position)
        {
            StartCoroutine(ShowLightningEffectCoroutine(position));
        }




        private IEnumerator ShowLightningEffectCoroutine(Vector3 position)
        {
            GameObject lightningEffect = Instantiate(lightningEffectPrefab, position, Quaternion.identity);
            yield return new WaitForSeconds(1f); // Duration of the effect
            Destroy(lightningEffect); // Remove the effect after 1 second
        }




        private void ConsumeChicken()
        {
            if (inventory.currentInventory != null && inventory.currentInventory == "chicken" && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Consuming chicken!");
                inventory.ResetInventory(); // Reset the inventory
                PV.RPC("IncreaseHealth", RpcTarget.All, 25);
            }
        }




        [PunRPC]
        private void IncreaseHealth(int amount)
        {
            health.currentHealth = Mathf.Min(health.currentHealth + amount, 100);
            Debug.Log($"Health increased by {amount}. Current health: {health.currentHealth}");
        }




        private void UseInvisibilityCloak()
        {
            if (inventory.currentInventory != null && inventory.currentInventory == "invisibilityCloak" && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Invisibility cloak activated!");
                inventory.ResetInventory(); // Reset the inventory




                // If there's an ongoing invisibility effect, stop it
                if (isInvisible)
                {
                    PV.RPC("DeactivateInvisibilityEffectWithoutDelay", RpcTarget.AllBuffered); // Deactivate current effect for all players
                }




                // Start a new invisibility effect
                PV.RPC("ActivateInvisibilityEffect", RpcTarget.AllBuffered);
            }
        }




        [PunRPC]
        private void ActivateInvisibilityEffect()
        {
            isInvisible = true; // Set the invisibility state to true
                                // Set local player opacity to 50%
            SetCharacterOpacity setOpacityScript = GetComponent<SetCharacterOpacity>();
            if (setOpacityScript != null)
            {
                setOpacityScript.SetOpacity();
            }




            // Make other players not see this character
            if (!PV.IsMine)
            {
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in meshRenderers)
                {
                    meshRenderer.enabled = false;
                }
            }




            invisibilityCoroutine = StartCoroutine(DeactivateInvisibilityEffectAfterDelay(10f));
        }




        private IEnumerator DeactivateInvisibilityEffectAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);




            PV.RPC("DeactivateInvisibilityEffectWithoutDelay", RpcTarget.AllBuffered);
        }




        [PunRPC]
        private void DeactivateInvisibilityEffectWithoutDelay()
        {
            isInvisible = false; // Set invisibility flag to false




            // Reset local player opacity to 100%
            SetCharacterOpacity setOpacityScript = GetComponent<SetCharacterOpacity>();
            if (setOpacityScript != null)
            {
                setOpacityScript.ResetOpacity();
            }




            // Make other players see this character again, but only if there's no new invisibility effect active
            if (!PV.IsMine)
            {
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in meshRenderers)
                {
                    meshRenderer.enabled = true;
                }
            }




            invisibilityCoroutine = null; // Reset the coroutine reference
        }




        private void ToggleCrosshair()
        {
            if (inventory.currentWeapon != null && inventory.currentWeapon != "")
            {
                crosshair.SetActive(true);
            }
            else
            {
                crosshair.SetActive(false);
            }
        }




        [PunRPC]
        private void RPC_SetVelocity(Vector3 initialVelocity)
        {
            rb.velocity = initialVelocity;
        }




        private void ShootLaserGun()
        {
            float projectileSpeed = 150f;
            if (Input.GetMouseButtonDown(0) && inventory.currentWeapon == "LaserGun")
            {




                if (laserProjectilePrefab != null && playerCamera != null)
                {
                    // Use the assigned camera to get the shoot direction
                    Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                    Vector3 shootDirection = ray.direction;




                    Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);




                    // Set the spawn position to be slightly in front of the player
                    Vector3 spawnPosition = transform.position + transform.forward * 1.5f;




                    // Calculate the rotation to align with the shoot direction
                    // Quaternion rotation = Quaternion.LookRotation(shootDirection);
                    // Debug.Log("rotation: " + rotation);




                    GameObject laserProjectile = PhotonNetwork.Instantiate("Prefabs/SingleLine-LightSaber Variant", spawnPosition, Quaternion.identity);




                    PhotonView projectilePV = laserProjectile.GetComponent<PhotonView>();
                    Rigidbody projectileRb = laserProjectile.GetComponent<Rigidbody>();




                    if (projectileRb != null)
                    {
                        projectileRb.velocity = shootDirection * projectileSpeed;




                        // Synchronize the projectile's velocity across all clients
                        projectilePV.RPC("RPC_SetVelocity", RpcTarget.AllBuffered, projectileRb.velocity);
                    }




                    // Start coroutine to destroy the projectile after 5 seconds
                    StartCoroutine(DestroyProjectileAfterDelay(laserProjectile, 5f));




                    // Reduce the number of shots remaining
                    shotsRemaining--;




                    // If no shots remaining, destroy the weapon and disable crosshair
                    if (shotsRemaining <= 0)
                    {
                        DestroyWeapon();
                    }
                }
            }
        }




        private void ShootRocketLauncher()
        {
            float projectileSpeed = 150f;
            if (Input.GetMouseButtonDown(0) && inventory.currentWeapon == "RocketLauncher")
            {




                if (rocketProjectilePrefab != null && playerCamera != null)
                {
                    // Use the assigned camera to get the shoot direction
                    Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                    Vector3 shootDirection = ray.direction;




                    Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);




                    // Set the spawn position to be slightly in front of the player
                    Vector3 spawnPosition = transform.position + transform.forward * 1.5f;




                    // Calculate the rotation to align with the shoot direction
                    // Quaternion rotation = Quaternion.LookRotation(shootDirection);
                    // Debug.Log("rotation: " + rotation);




                    GameObject rocketProjectile = PhotonNetwork.Instantiate("Prefabs/Rocket_Launcher_Missile", spawnPosition, Quaternion.identity);




                    PhotonView projectilePV = rocketProjectile.GetComponent<PhotonView>();
                    Rigidbody projectileRb = rocketProjectile.GetComponent<Rigidbody>();




                    if (projectileRb != null)
                    {
                        projectileRb.velocity = shootDirection * projectileSpeed;




                        // Synchronize the projectile's velocity across all clients
                        projectilePV.RPC("RPC_SetVelocityRocketProjectile", RpcTarget.AllBuffered, projectileRb.velocity);
                    }




                    // Start coroutine to destroy the projectile after 5 seconds
                    StartCoroutine(DestroyProjectileAfterDelay(rocketProjectile, 5f));




                    // Reduce the number of shots remaining
                    shotsRemaining--;




                    // If no shots remaining, destroy the weapon and disable crosshair
                    if (shotsRemaining <= 0)
                    {
                        DestroyWeapon();
                    }
                }
            }
        }




        private IEnumerator DestroyProjectileAfterDelay(GameObject projectile, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (projectile != null)
            {
                PhotonNetwork.Destroy(projectile);
            }
        }




        [PunRPC]
        private void RPC_DisableLaserGun()
        {
            DisableLaserGun();
        }




        [PunRPC]
        private void RPC_DisableRocketLauncher()
        {
            DisableRocketLauncher();
        }








        private void DestroyWeapon()
        {
            // Optionally, you might want to notify other scripts or update the UI here




            // Disable crosshair
            crosshair.SetActive(false);




            // Reset weapon slot or destroy the weapon object
            inventory.currentWeapon = null; // Or other logic to handle weapon removal




            // Optionally, you might want to destroy the weapon object or disable it
            // For example: Destroy(gameObject);
            photonView.RPC("RPC_DisableLaserGun", RpcTarget.AllBuffered);
            shotsRemaining = 3; // Reset the number of shots remaining
        }




        private void DisableLaserGun()
        {
            sciFiRifle.SetActive(false);


        }




        private void DisableRocketLauncher()
        {
            rocketLauncher.SetActive(false);
        }








        // private void OnCollisionEnter(Collision collision)
        // {
        //     if (collision.gameObject.CompareTag("Player"))
        //     {
        //         PhotonView targetPV = collision.gameObject.GetComponent<PhotonView>();
        //         if (targetPV != null && targetPV.IsMine == false)
        //         {
        //             targetPV.RPC("TakeDamage", RpcTarget.All, 35); // Adjust the damage value as needed
        //         }




        //         // Destroy the projectile after it hits something
        //         PhotonNetwork.Destroy(gameObject);
        //     }
        // }




    }
}





