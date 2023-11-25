using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;

        int vertical;
        int horizontal;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            float horizontalAmount = horizontalValue;
            float verticalAmount = verticalValue;
            
            if (isSprinting)
            {
                verticalAmount = 2;
            }
            character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);

        }

        public virtual void PlayTargetActionAnimation(string targetActionAnimation, 
           bool isPerformingAction,
           bool applyRootMotion = true, 
           bool canRotate = false, 
           bool canMove = false)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetActionAnimation, .2f);

            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;

            character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetActionAnimation, isPerformingAction, applyRootMotion, canMove, canRotate);
        }

    public virtual void PlayTargetAttackActionAnimation(string targetActionAnimation,
       bool isPerformingAction,
       bool applyRootMotion = true,
       bool canRotate = false,
       bool canMove = false)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetActionAnimation, .2f);

        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;

        character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetActionAnimation, isPerformingAction, applyRootMotion, canMove, canRotate);
    }
}