using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager playerManager;
        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2f;
        [SerializeField] float runningSpeed = 6f;
        [SerializeField] float sprintingSpeed = 10f;
        [SerializeField] float rotationSpeed = 10f;
        [SerializeField] float sprintingStaminaCost = 5f;

        [Header("Jump")]
        private Vector3 jumpDirection;
        [SerializeField] float jumpHeight = 4f;
        [SerializeField] float jumpStaminaCost = 15f;
        [SerializeField] float jumpForwardSpeed = 5f;
        [SerializeField] float freeFallSpeed = 2f;

        [Header("Dodge")]
        private Vector3 rollDirection;
        [SerializeField] float dodgeStaminaCost = 25f;


        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();
            if (playerManager.IsOwner)
            {
                playerManager.characterNetworkManager.animatorVerticalParameter.Value = verticalMovement;
                playerManager.characterNetworkManager.animatorHorizontalParameter.Value = horizontalMovement;
                playerManager.characterNetworkManager.networkMoveAmount.Value = moveAmount;
            }
            else
            {
                moveAmount = playerManager.characterNetworkManager.networkMoveAmount.Value;
                verticalMovement = playerManager.characterNetworkManager.animatorVerticalParameter.Value;
                horizontalMovement = playerManager.characterNetworkManager.animatorHorizontalParameter.Value;

                playerManager.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, playerManager.playerNetworkManager.isSprinting.Value);
            }
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            HandleJumpingMovement();
            HandleFreeFallMovement();
        }

        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.playerVerticalInput;
            horizontalMovement = PlayerInputManager.instance.playerHorizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
        }

        private void HandleGroundedMovement()
        {
            if (!playerManager.canMove) return;

            GetMovementValues();
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (playerManager.playerNetworkManager.isSprinting.Value)
            {
                playerManager.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else
            {
                if (PlayerInputManager.instance.moveAmount > .5f)
                {
                    playerManager.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (PlayerInputManager.instance.moveAmount <= .5f)
                {
                    playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }
        }

        private void HandleJumpingMovement()
        {
            if (playerManager.playerNetworkManager.isJumping.Value)
            {
                playerManager.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
            }
        }

        private void HandleFreeFallMovement()
        {
            if (!playerManager.isGrounded)
            {
                Vector3 freeFallDirection;
                freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.playerVerticalInput;
                freeFallDirection = freeFallDirection + PlayerCamera.instance.transform.right * PlayerInputManager.instance.playerHorizontalInput;
                freeFallDirection.y = 0;
                freeFallDirection.Normalize();

                playerManager.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            if (!playerManager.canRotate) return;

            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }

        public void HandleSprinting()
        {
            if (playerManager.isPerformingAction)
            {
                playerManager.playerNetworkManager.isSprinting.Value = false;
                return;
            }
            if(playerManager.playerNetworkManager.currentStamina.Value <= 0)
            {
                playerManager.playerNetworkManager.isSprinting.Value = false;
                return;
            }
            if(moveAmount >= 0.5)
            {
                playerManager.playerNetworkManager.isSprinting.Value = true;
            }
            else
            {
                playerManager.playerNetworkManager.isSprinting.Value = false;
            }

            if (playerManager.playerNetworkManager.isSprinting.Value)
            {
                playerManager.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
            }
        }

        public void AttemptToPerformDodge()
        {
            if (playerManager.isPerformingAction) return;

            if(playerManager.playerNetworkManager.currentStamina.Value <= 0) return;

            if (PlayerInputManager.instance.moveAmount > 0)
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.playerVerticalInput;
                rollDirection = rollDirection + PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.playerHorizontalInput;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                playerManager.transform.rotation = playerRotation;

                playerManager.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward", true, true);
            }
            else
            {
                playerManager.playerAnimatorManager.PlayTargetActionAnimation("Back_Step", true, true);
            }

            playerManager.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
        }

        public void AttemptToPerformJump()
        {
            if (playerManager.isPerformingAction) return;
            if (playerManager.playerNetworkManager.currentStamina.Value <= 0) return;
            if (playerManager.playerNetworkManager.isJumping.Value) return;
            if (!playerManager.isGrounded) return;

            playerManager.playerAnimatorManager.PlayTargetActionAnimation("Jumping_Up", false);

            playerManager.playerNetworkManager.isJumping.Value = true;

            playerManager.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

            jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.playerVerticalInput;
            jumpDirection = jumpDirection + PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.playerHorizontalInput;
            jumpDirection.y = 0;
            jumpDirection.Normalize();

            if(jumpDirection != Vector3.zero)
            {
                if (playerManager.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1f;
                }
                else if (PlayerInputManager.instance.moveAmount > 0.5)
                {
                    jumpDirection *= 0.5f;
                }
                else if (PlayerInputManager.instance.moveAmount <= 0.5)
                {
                    jumpDirection *= 0.25f;
                }
            }
        }

        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
        }
    }