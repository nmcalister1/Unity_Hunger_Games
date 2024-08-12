using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Events;


namespace RPG.Character
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject cameraHolder;
        [SerializeField] private float mouseSensitivity = 2f, walkSpeed = 4f, jumpForce = 5f, smoothTime = 0.1f, sprintSpeed = 5f;
        [SerializeField] private GameObject fastFeetEffectPrefab; // Reference to the Fast Feet effect prefab
        [SerializeField] private GameObject lightningEffectPrefab; // Reference to the Lightning effect prefab
        [SerializeField] private GameObject healingEffectPrefab; // Reference to the Invisibility effect prefab
        [SerializeField] private GameObject sciFiRifle;




        [SerializeField] private GameObject rocketLauncher;
        [SerializeField] private GameObject mine;
        public GameObject landMinePrefab;
        public GameObject swordPrefab;
        public GameObject crosshair;
        public Transform crosshairSpawnPoint;





        public float recoilForce = 10f;


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
        private bool isFastFeetActive; // To keep track of the fast feet state


        public TextMeshProUGUI newSuppliesText;
        public TextMeshProUGUI lowHealthText;
        public static UnityEvent OnNewSuppliesEvent = new UnityEvent();

        private const byte NEW_SUPPLIES_EVENT = 1;


        private bool isLow = false;
        private bool isCoroutineRunning = false;

        private float deathHeight = -1f;


        public GameObject laserProjectilePrefab;
        public GameObject rocketProjectilePrefab;
        // public Transform projectileSpawnPoint;
        private int shotsRemaining = 3;
        private int minesRemaining = 3;
        private int swingsRemaining = 3;
        private float swordSwingRadius = 3f; // Radius of the sword swing
        private int swordDamage = 50;
        private bool canMove = true;

        private PlayerUIManager playerUIManager;

        private int ownerNumber;
        private List<int> playerActorNumbers = new List<int>();





        private HashSet<Collider> processedColliders = new HashSet<Collider>();




        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            PV = GetComponent<PhotonView>();
            animator = GetComponentInChildren<Animator>();
            health = GetComponent<Health>(); // Get the Health component
            inventory = GetComponent<Inventory>(); // Get the Inventory component
            playerUIManager = GetComponent<PlayerUIManager>();
            originalSpeed = walkSpeed; // Store the original walking speed
            ownerNumber = 1;

        }




        private void Start()
        {
            if (!PV.IsMine)
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(rb);
                
            }
            else 
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(DecreaseHealthPeriodically());
                    StartCountdownTimer();
                }
            
            }
            
        }




        private void Update()
        {
            if (!PV.IsMine) return;
            if (!canMove) return;
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
            UseLandMine();
            UseSword();

            CheckHealth();
            CheckFallOffMap();

            //CheckActorNumber();

        }

        public void StartCountdownTimer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                PhotonView playerPhotonView = player.GetComponent<PhotonView>();
                if (playerPhotonView != null)
                {
                    playerPhotonView.RPC("DisableMovementDuringCountdownRPC", RpcTarget.AllBuffered);
                }
            }
            
        }

        [PunRPC]
        private void DisableMovementDuringCountdownRPC()
        {
            DisableMovementDuringCountdown();
        }

         [PunRPC]
        public void EnableMovementRPC()
        {
            EnableMovement();
        }

        private void DisableMovementDuringCountdown()
        {
            CountdownTimer countdownTimer = GetComponent<CountdownTimer>();
            //PlayerController playerController = GetComponent<PlayerController>();
            if (countdownTimer != null)
            {
                countdownTimer.enabled = true;
                StartCoroutine(WaitForCountdown());
                //yield return new WaitUntil(() => countdownTimer.IsCountdownFinished());
                //Debug.LogWarning("Countdown finished. Enabling movement.");
                //PV.RPC("EnableMovementRPC", RpcTarget.AllBuffered);
            }
        }

        private IEnumerator WaitForCountdown()
        {
            yield return new WaitForSeconds(3f);
            DestroySpawnPointsRPC();
            
        }

        //[PunRPC]
        private void DestroySpawnPointsRPC()
        {
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
            foreach (GameObject spawnPoint in spawnPoints)
            {
                Destroy(spawnPoint);
            }
        }

        public void EnableMovement()
        {
            Debug.Log("Movement enabled for player: " + PV.Owner.NickName);
            canMove = true;
        }

        public void DisableMovement()
        {
            canMove = false;
        }

        private new void OnEnable()
        {
            OnNewSuppliesEvent.AddListener(OnNewSupplies);
        }

        private new void OnDisable()
        {
            OnNewSuppliesEvent.RemoveListener(OnNewSupplies);
        }

        public void OnNewSupplies()
        {
            Debug.Log("New supplies event has been triggered.");
            photonView.RPC("StartNewSuppliesEvent", RpcTarget.All);
        }

        [PunRPC]
        private void StartNewSuppliesEvent()
        {
            StartCoroutine(ShowNewSuppliesText());
        }

        private IEnumerator ShowNewSuppliesText()
        {
            if (newSuppliesText != null)
            {
                Debug.Log("Showing new supplies text.");
                newSuppliesText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                newSuppliesText.gameObject.SetActive(false);
            }
        }

        private void CheckHealth()
        {
            if (!isLow && health.currentHealth > 20) return; // Exit if health is fine and not low
            if (isLow && health.currentHealth <= 20) return; // Exit if health is still low
            if (isLow && health.currentHealth > 20)
            {
                isLow = false;
                lowHealthText.gameObject.SetActive(false);
                return; // Exit after resetting low health state
            }

            isLow = true;
            if (!isCoroutineRunning)
            {
                lowHealthText.gameObject.SetActive(true);
                StartCoroutine(ShowLowHealthText());
            }
        }

        private IEnumerator ShowLowHealthText()
        {
            isCoroutineRunning = true;
            yield return new WaitForSeconds(2f);
            lowHealthText.gameObject.SetActive(false);
            isCoroutineRunning = false;
        }

        private void CheckFallOffMap()
        {
            if (transform.position.y < deathHeight)
            {
                photonView.RPC("TakeDamage", RpcTarget.AllBuffered, 100);
            }
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

        public void ChickenAtStart()
        {
            photonView.RPC("GetChickenAtStart", RpcTarget.All);
        }

        [PunRPC]
        public void GetChickenAtStart()
        {
            inventory.PickUpItem("chicken");
        }



        [PunRPC]
        public void TakeDamage(int damage)
        {
            //if (!PV.IsMine) return;

            // Reduce the player's health
            health.currentHealth -= damage;

            health.currentHealth = Mathf.Clamp(health.currentHealth, 0, 100);

            //health.SyncHealthHealth();
            //photonView.RPC("UpdatePlayerHealth", RpcTarget.All, photonView.Owner.ActorNumber, (float)health.currentHealth);    

            if (health.currentHealth <= 0)
            {
                // Handle player death
                Die();
            }
            
        }

        private IEnumerator DecreaseHealthPeriodically()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                DecreaseAllPlayersHealth();
            }
        }

        private void DecreaseAllPlayersHealth()
        {
            // photonView.RPC("TakeDamage", RpcTarget.All, 5);
            // ChickenAtStart();
            // Find all GameObjects with the "Player" tag
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                Debug.Log("Player: " + player.name);
                PhotonView playerPhotonView = player.GetComponent<PhotonView>();
                
                if (playerPhotonView != null)
                {
                    // Call the TakeDamage method on each player's PhotonView
                    playerPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, 5);
                }
            }
        }





        private void Die()
        {
            // You can add logic to handle what happens when the player dies
            Debug.Log("Player has died!");

            // Call RPC to hide the player for all clients
            StartCoroutine(WaitAndHidePlayer());
            //health.SyncHealthHealth();
            //photonView.RPC("RPC_HidePlayer", RpcTarget.AllBuffered);
        }

        private IEnumerator WaitAndHidePlayer()
        {
            yield return new WaitForSeconds(.1f);
            photonView.RPC("RPC_HidePlayer", RpcTarget.AllBuffered);
        }

        private void Die(int playerID)
        {
            PhotonView playerView = PhotonView.Find(playerID);
            if (playerView != null && playerView.TryGetComponent(out PlayerController playerController))
            {
                health.SyncHealthHealth();
                playerController.photonView.RPC("RPC_HidePlayer", RpcTarget.AllBuffered);
            }
        }


        [PunRPC]
        private void RPC_HidePlayer()
        {
            // Hide the player by disabling the game object
            gameObject.SetActive(false);
            // PhotonNetwork.Destroy(gameObject);
            // PhotonNetwork.LeaveRoom();
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
                minesRemaining = 3;
                swingsRemaining = 3;
                EnableLaserGun();
                photonView.RPC("RPC_EnableLaserGun", RpcTarget.AllBuffered);
            }




            if (itemName == "RocketLauncher")
            {
                shotsRemaining = 3; // Set the number of shots remaining
                minesRemaining = 3;
                swingsRemaining = 3;
                EnableRocketLauncher();
                photonView.RPC("RPC_EnableRocketLauncher", RpcTarget.AllBuffered);

            }




            if (itemName == "Mines")
            {
                shotsRemaining = 3; // Set the number of shots remaining
                minesRemaining = 3;
                swingsRemaining = 3;
                EnableMines();
                photonView.RPC("RPC_EnableMines", RpcTarget.AllBuffered);

            }




            if (itemName == "Weapon")
            {
                shotsRemaining = 3; // Set the number of shots remaining
                minesRemaining = 3;
                swingsRemaining = 3;
                EnableWeapon();
                photonView.RPC("RPC_EnableWeapon", RpcTarget.AllBuffered);

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

        private void EnableWeapon()
        {
            if (swordPrefab != null)
            {
                swordPrefab.SetActive(true);
            }
        }




        [PunRPC]
        private void RPC_EnableWeapon()
        {
            EnableWeapon();
        }


        private void EnableMines()
        {
            if (mine != null)
            {
                mine.SetActive(true);
            }
        }




        [PunRPC]
        private void RPC_EnableMines()
        {
            EnableMines();
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

        private void UseLandMine()
        {
            if (inventory.currentWeapon != null && inventory.currentWeapon == "Mines" && Input.GetMouseButtonDown(0) && minesRemaining > 0)
            {
                Debug.Log("Land Mine activated!");
                PlaceLandMine();
            }
        }

        private void PlaceLandMine()
        {
            if (!photonView.IsMine) return;

            // Get the position in front of the player to place the land mine
            Vector3 minePosition = transform.position + transform.forward * 1.5f;
            
            // Instantiate the land mine using Photon
            PhotonNetwork.Instantiate("Prefabs/LandMine Variant 1", minePosition, Quaternion.identity);

            // Reduce the number of shots remaining
            minesRemaining--;

            // If no shots remaining, destroy the weapon and disable crosshair
            if (minesRemaining <= 0)
            {
                DestroyWeapon();
            }
        }

        private void UseSword()
        {
            if (inventory.currentWeapon != null && inventory.currentWeapon == "Weapon" && Input.GetMouseButtonDown(0) && swingsRemaining > 0)
            {
                Debug.Log("Sword activated!");

                animator.SetTrigger("attack"); 
                // Check for players in the swing radius
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, swordSwingRadius);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Player"))
                    {
                        PhotonView targetPV = hitCollider.GetComponent<PhotonView>();
                        if (targetPV != null && !targetPV.IsMine)
                        {
                            Debug.Log("Player in range: " + hitCollider.gameObject.name);
                            // Apply damage to the player
                            targetPV.RPC("TakeDamage", RpcTarget.All, swordDamage);
                        }
                    }
                }

                // Reduce the number of swings left
                swingsRemaining--;

                // Optionally, you can add logic to disable the sword or show a cooldown
                if (swingsRemaining <= 0)
                {
                    // Handle out-of-swings logic
                    StartCoroutine(DestroyWeaponWithDelay());
                }
                
            }
        }

        // Coroutine to destroy the weapon after a 1-second delay
        private IEnumerator DestroyWeaponWithDelay()
        {
            yield return new WaitForSeconds(0.7f);
            DestroyWeapon();
        }




        private void UseFastFeet()
        {
            if (inventory.currentInventory != null && inventory.currentInventory == "fastFeet" && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Fast feet activated!");
                inventory.ResetInventory(); // Reset the inventory

                // Start a new fast feet effect
                PV.RPC("ActivateFastFeetEffect", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        private void ActivateFastFeetEffect()
        {
            // If there's an ongoing fast feet effect, stop it
            if (isFastFeetActive && fastFeetCoroutine != null)
            {
                StopCoroutine(fastFeetCoroutine);
                DeactivateFastFeetEffectWithoutDelay();
            }

            // Set the fast feet state to true and instantiate the effect
            isFastFeetActive = true;
            if (currentEffect != null) Destroy(currentEffect);
            currentEffect = Instantiate(fastFeetEffectPrefab, transform.position, Quaternion.identity, transform);

            // Increase speed and start the coroutine to deactivate after delay
            walkSpeed = 7.5f;
            fastFeetCoroutine = StartCoroutine(DeactivateFastFeetEffectAfterDelay(10f));
        }

        private IEnumerator DeactivateFastFeetEffectAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            PV.RPC("DeactivateFastFeetEffectWithoutDelay", RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void DeactivateFastFeetEffectWithoutDelay()
        {
            isFastFeetActive = false;

            // Reset speed to original value and remove the effect
            walkSpeed = originalSpeed;
            if (currentEffect != null) Destroy(currentEffect);
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
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f);
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
            yield return new WaitForSeconds(0.7f); // Duration of the effect
            Destroy(lightningEffect); // Remove the effect after 1 second
        }




        private void ConsumeChicken()
        {
            if (inventory.currentInventory != null && inventory.currentInventory == "chicken" && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Consuming chicken!");
                inventory.ResetInventory(); // Reset the inventory
                PV.RPC("IncreaseHealth", RpcTarget.All, 35);
                PV.RPC("ShowHealingEffect", RpcTarget.All, transform.position);
            }
        }

        [PunRPC]
        private void ShowHealingEffect(Vector3 position)
        {
            StartCoroutine(ShowHealingEffectCoroutine(position));
        }

        private IEnumerator ShowHealingEffectCoroutine(Vector3 position)
        {
            GameObject healingEffect = Instantiate(healingEffectPrefab, position, Quaternion.identity);
            yield return new WaitForSeconds(0.7f); // Duration of the effect
            Destroy(healingEffect); // Remove the effect after 1 second
        }




        [PunRPC]
        private void IncreaseHealth(int amount)
        {
            health.currentHealth = Mathf.Min(health.currentHealth + amount, 100);
            photonView.RPC("UpdatePlayerHealth", RpcTarget.All, photonView.Owner.ActorNumber, (float)health.currentHealth);

            //playerUIManager.UpdatePlayerHealth(photonView.Owner.ActorNumber, health.currentHealth);

            //photonView.RPC("RPC_UpdateHealth", RpcTarget.All, photonView.Owner.ActorNumber, health.currentHealth);
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
                setOpacityScript.ShowInvisibilityMessage();
            }
            //PV.RPC("SetOpacity", RpcTarget.AllBuffered);




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


            SetCharacterOpacity setOpacityScript = GetComponent<SetCharacterOpacity>();

            if (setOpacityScript != null)
            {
                setOpacityScript.ShowVisibleMessage();
            }


            PV.RPC("DeactivateInvisibilityEffectWithoutDelay", RpcTarget.AllBuffered);
        }




        [PunRPC]
        private void DeactivateInvisibilityEffectWithoutDelay()
        {
            isInvisible = false; // Set invisibility flag to false




            //Reset local player opacity to 100%
            //SetCharacterOpacity setOpacityScript = GetComponent<SetCharacterOpacity>();
            // if (setOpacityScript != null)
            // {
            //     setOpacityScript.ResetOpacity();
            // }

            //PV.RPC("ResetOpacity", RpcTarget.AllBuffered);




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
            if (inventory.currentWeapon != null && inventory.currentWeapon != "" && inventory.currentWeapon != "Mines" && inventory.currentWeapon != "Weapon")
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
            if (Input.GetMouseButtonDown(0) && inventory.currentWeapon == "LaserGun" && shotsRemaining > 0)
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
            if (Input.GetMouseButtonDown(0) && inventory.currentWeapon == "RocketLauncher" && shotsRemaining > 0)
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




                    GameObject rocketProjectile = PhotonNetwork.Instantiate("Prefabs/SingleLine-LightSaber Variant 1", spawnPosition, Quaternion.identity);




                    PhotonView projectilePV = rocketProjectile.GetComponent<PhotonView>();
                    Rigidbody projectileRb = rocketProjectile.GetComponent<Rigidbody>();




                    if (projectileRb != null)
                    {
                        projectileRb.velocity = shootDirection * projectileSpeed;




                        // Synchronize the projectile's velocity across all clients
                        projectilePV.RPC("RPC_SetVelocityRocketProjectile", RpcTarget.AllBuffered, projectileRb.velocity);

                        // Apply recoil to the player and synchronize it
                        ApplyRecoil(-shootDirection);
                        PV.RPC("RPC_ApplyRecoil", RpcTarget.Others, -shootDirection * recoilForce);
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

        [PunRPC]
        private void RPC_ApplyRecoil(Vector3 recoilForce)
        {
            rb.AddForce(recoilForce, ForceMode.Impulse);
        }

        private void ApplyRecoil(Vector3 recoilDirection)
        {
            rb.AddForce(recoilDirection * recoilForce, ForceMode.Impulse);
        }




        private IEnumerator DestroyProjectileAfterDelay(GameObject projectile, float delay)
        {
            yield return new WaitForSeconds(delay);
            // if (projectile != null)
            // {
            //     PhotonNetwork.Destroy(projectile);
            // }
            // if(projectile != null && (projectile.GetComponent<PhotonView>().IsMine || PhotonNetwork.IsMasterClient))
            // {
            //     PhotonNetwork.Destroy(projectile);
            // }
            if (projectile != null)
            {
                PhotonView projectilePV = projectile.GetComponent<PhotonView>();

                Debug.Log($"DestroyProjectileAfterDelay called by: {PhotonNetwork.NickName}, IsMine: {projectilePV.IsMine}, IsMasterClient: {PhotonNetwork.IsMasterClient}");

                if (projectilePV.IsMine || PhotonNetwork.IsMasterClient)
                {
                    Debug.Log($"Destroying projectile after delay by: {PhotonNetwork.NickName}, IsMine: {projectilePV.IsMine}, IsMasterClient: {PhotonNetwork.IsMasterClient}");
                    PhotonNetwork.Destroy(projectile);
                }
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

        [PunRPC]
        private void RPC_DisableMines()
        {
            DisableMines();
        }

        [PunRPC]
        private void RPC_DisableWeapon()
        {
            DisableWeapon();
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
            photonView.RPC("RPC_DisableRocketLauncher", RpcTarget.AllBuffered);
            photonView.RPC("RPC_DisableMines", RpcTarget.AllBuffered);
            photonView.RPC("RPC_DisableWeapon", RpcTarget.AllBuffered);
            shotsRemaining = 3; // Reset the number of shots remaining
            minesRemaining = 3;
            swingsRemaining = 3;
        }




        private void DisableLaserGun()
        {
            sciFiRifle.SetActive(false);
        }

        private void DisableWeapon()
        {
            swordPrefab.SetActive(false);
        }


        private void DisableRocketLauncher()
        {
            rocketLauncher.SetActive(false);
        }

        private void DisableMines()
        {
            mine.SetActive(false);
        }


    }
}





