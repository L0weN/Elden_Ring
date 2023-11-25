using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        public PlayerManager player;
        PlayerControls playerControls;

        [Header("Player Movement Input")]
        [SerializeField] Vector2 playerInput;
        public float playerHorizontalInput;
        public float playerVerticalInput;
        public float moveAmount;

        [Header("Camera Movement Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraHorizontalInput;
        public float cameraVerticalInput;

        [Header("Player Action Input")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput = false;
        [SerializeField] bool jumpInput = false;
        [SerializeField] bool RB_Input = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnSceneChanged;

            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            else
            {
                instance.enabled = false;
                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                // Moving the player will trigger the movement action
                playerControls.PlayerMovement.Movement.performed += ctx => playerInput = ctx.ReadValue<Vector2>();
                // Moving the camera will trigger the camera movement action
                playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
                // Pressing the dodge button will trigger the dodge action
                playerControls.PlayerActions.Dodge.performed += ctx => dodgeInput = true;
                // Pressing the jump button will trigger the jump action
                playerControls.PlayerActions.Jump.performed += ctx => jumpInput = true;
                // Holding down the sprint button will trigger the sprint action
                playerControls.PlayerActions.Sprint.performed += ctx => sprintInput = true;
                // Releasing the sprint button will trigger the sprint action
                playerControls.PlayerActions.Sprint.canceled += ctx => sprintInput = false;
                // Pressing the RB button will trigger the RB action
                playerControls.PlayerActions.RB.performed += ctx => RB_Input = true;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        
        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleJumpInput();
            HandleSprintInput();
            HandleRBInput();
        }
        // Movement
        private void HandlePlayerMovementInput()
        {
            playerHorizontalInput = playerInput.x;
            playerVerticalInput = playerInput.y;

            moveAmount = Mathf.Clamp01(Mathf.Abs(playerHorizontalInput) + Mathf.Abs(playerVerticalInput));

            if (moveAmount <= .5f && moveAmount > 0f)
            {
                moveAmount = .5f;
            }
            else if (moveAmount > .5f && moveAmount <= 1f)
            {
                moveAmount = 1f;
            }
            if (player == null) return;
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
        }

        private void HandleCameraMovementInput()
        {
            cameraHorizontalInput = cameraInput.x;
            cameraVerticalInput = cameraInput.y;
        }

        // Actions
        private void HandleDodgeInput()
        {
            if (dodgeInput)
            {
                dodgeInput = false;
                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleSprintInput()
        {
            if (sprintInput)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if (jumpInput)
            {
                jumpInput = false;
                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }

        private void HandleRBInput()
        {
            if (RB_Input)
            {
                RB_Input = false;
                player.playerNetworkManager.SetCharacterActionHand(true);
                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RB_Action, player.playerInventoryManager.currentRightHandWeapon);
            }
        }
    }