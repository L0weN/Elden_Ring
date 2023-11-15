using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTER
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager characterManager;
        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -40f;
        [SerializeField] LayerMask groundCheckLayerMask;
        [SerializeField] float groundCheckSphereRadius = 0.25f;
        [SerializeField] protected Vector3 yVelocity;
        [SerializeField] protected float groundedYVelocity = -20f;
        [SerializeField] protected float fallStartYVelocity = -5f;
        protected bool fallingVelocityHasBeenSet = false;
        protected float inAirTimer = 0;


        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (characterManager.isGrounded)
            {
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityHasBeenSet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                if (!characterManager.isJumping && !fallingVelocityHasBeenSet)
                {
                    fallingVelocityHasBeenSet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer += Time.deltaTime;
                characterManager.animator.SetFloat("inAirTimer", inAirTimer);

                yVelocity.y += gravityForce * Time.deltaTime;

                
            }

            characterManager.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            characterManager.isGrounded = Physics.CheckSphere(characterManager.transform.position, groundCheckSphereRadius, groundCheckLayerMask);
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(characterManager.transform.position, groundCheckSphereRadius);
        }
    }
}