using UnityEngine;
using Photon.Pun;
using RPG.Utility;
using System;

namespace RPG.Character
{
    public class Movement : MonoBehaviour
    {
        private PhotonView PV;
        [NonSerialized] public bool isMoving = false;
        private Animator animatorCmp;
        //private float currentSpeed = 0f;
        private float acceleration = 2f; // Customize this value as needed
        private float deceleration = 2f; // Customize this value as needed

        private void Awake()
        {
            PV = GetComponent<PhotonView>();
            animatorCmp = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (!PV.IsMine)
            {
                return;
            }

            HandleMovementInput();
            MovementAnimator();
        }

        [PunRPC]
        private void UpdateAnimationState(bool moving)
        {
            // Debug.Log($"[RPC] UpdateAnimationState called with moving: {moving} on {PV.ViewID}");
            isMoving = moving;
            MovementAnimator();
        }

        private void HandleMovementInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Debug.Log($"[HandleMovementInput] Horizontal: {horizontal}, Vertical: {vertical} on {PV.ViewID}");

            if (horizontal != 0 || vertical != 0)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            PV.RPC("UpdateAnimationState", RpcTarget.All, isMoving);
        }

        private void MovementAnimator()
        {
            float speed = animatorCmp.GetFloat(Constants.SPEED_ANIMATOR_PARAM);
            float smoothening = Time.deltaTime * (isMoving ? acceleration : deceleration);

            if (isMoving)
            {
                speed += smoothening;
            }
            else
            {
                speed -= smoothening;
            }

            speed = Mathf.Clamp01(speed);

            animatorCmp.SetFloat(Constants.SPEED_ANIMATOR_PARAM, speed);
        }
    }
}

