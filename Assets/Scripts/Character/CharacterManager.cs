using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public Animator animator;

        [Header("Flags")]
        public bool isGrounded = true;
        public bool isPerformingAction = false;
        public bool applyRootMotion = false;
        public bool canRotate = true;
        public bool canMove = true;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);

            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            IgnoreMyOwnColliders();
        }

        protected virtual void Update()
        {
            animator.SetBool("isGrounded", isGrounded);
        }

        protected virtual void LateUpdate()
        {
            
        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;

                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Death", true);
                }
            }
            yield return new WaitForSeconds(5f);
        }

        public virtual void ReviveCharacter()
        {

        }

        protected virtual void IgnoreMyOwnColliders()
        {
            // Character Controller Collider
            Collider characterControllerCollider = GetComponent<Collider>();

            // Damageable Character Layer Colliders
            Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();

            // List of colliders to ignore
            List<Collider> ignoreColliders = new List<Collider>();

            // Add all of our damageable colliders to the ignore list
            foreach (var collider in damageableCharacterColliders)
            {
                ignoreColliders.Add(collider);
            }

            // Add our character controller collider to the ignore list
            ignoreColliders.Add(characterControllerCollider);

            // Ignore all of the colliders in the ignore list
            foreach (var collider in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    Physics.IgnoreCollision(collider, otherCollider, true);
                }
            }
        }
    }