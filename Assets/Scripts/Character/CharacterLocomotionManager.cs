using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager character;
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
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (character.isGrounded)
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
                if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
                {
                    fallingVelocityHasBeenSet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer += Time.deltaTime;
                character.animator.SetFloat("inAirTimer", inAirTimer);

                yVelocity.y += gravityForce * Time.deltaTime;

                
            }

            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundCheckLayerMask);
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }
    }