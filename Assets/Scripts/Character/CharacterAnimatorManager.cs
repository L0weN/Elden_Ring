using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace CHARACTER
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager characterManager;

        int vertical;
        int horizontal;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
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
            characterManager.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            characterManager.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);

        }

        public virtual void PlayTargetActionAnimation(string targetActionAnimation, 
           bool isPerformingAction, 
           bool applyRootMotion = true, 
           bool canRotate = false, 
           bool canMove = false)
        {
            characterManager.applyRootMotion = applyRootMotion;
            characterManager.animator.CrossFade(targetActionAnimation, .2f);
            characterManager.isPerformingAction = isPerformingAction;
            characterManager.canRotate = canRotate;
            characterManager.canMove = canMove;

            characterManager.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetActionAnimation, isPerformingAction, applyRootMotion, canMove, canRotate);
        }
    }
}