using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace CHARACTER
{
    public class CharacterManager : NetworkBehaviour
    {
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public Animator animator;

        [Header("Flags")]
        public bool isGrounded = true;
        public bool isJumping = false;
        public bool isPerformingAction = false;
        public bool applyRootMotion = false;
        public bool canRotate = true;
        public bool canMove = true;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);

            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            animator = GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            animator.SetBool("isGrounded", isGrounded);
        }

        protected virtual void LateUpdate()
        {
            
        }

        
    }
}