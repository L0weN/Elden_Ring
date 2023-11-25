using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager character;

        [Header("Animator")]
        public NetworkVariable<float> animatorHorizontalParameter = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> animatorVerticalParameter = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> networkMoveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Stats")]
        public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [Header("Resources")]
        public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public void CheckHP(float oldValue, float newValue)
        {
            if (currentHealth.Value <= 0)
            {
                StartCoroutine(character.ProcessDeathEvent());
            }

            if (character.IsOwner)
            {
                if(currentHealth.Value > maxHealth.Value)
                {
                    currentHealth.Value = maxHealth.Value;
                }
            }
        }

        

        [ServerRpc]
        public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool isPerformingAction, bool applyRootMotion, bool canMove, bool canRotate)
        {
            if (IsServer)
            {
                PlayActionAnimationForAllClientsClientRpc(clientID, animationID, isPerformingAction, applyRootMotion, canMove, canRotate);
            }
        }

        [ClientRpc]
        public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool isPerformingAction, bool applyRootMotion, bool canMove, bool canRotate)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformActionAnimationFromServer(animationID, isPerformingAction, applyRootMotion, canMove, canRotate);
            }
        }

        private void PerformActionAnimationFromServer(string animationID, bool isPerformingAction, bool applyRootMotion, bool canMove, bool canRotate)
        {
            character.isPerformingAction = isPerformingAction;
            character.applyRootMotion = applyRootMotion;
            character.canMove = canMove;
            character.canRotate = canRotate;
            character.animator.CrossFade(animationID, .2f);
        }

        // Attack Action

        [ServerRpc]
        public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string animationID, bool isPerformingAction, bool applyRootMotion, bool canMove, bool canRotate)
        {
            if (IsServer)
            {
                PlayAttackActionAnimationForAllClientsClientRpc(clientID, animationID, isPerformingAction, applyRootMotion, canMove, canRotate);
            }
        }

        [ClientRpc]
        public void PlayAttackActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool isPerformingAction, bool applyRootMotion, bool canMove, bool canRotate)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformAttackActionAnimationFromServer(animationID, isPerformingAction, applyRootMotion, canMove, canRotate);
            }
        }

        private void PerformAttackActionAnimationFromServer(string animationID, bool isPerformingAction, bool applyRootMotion, bool canMove, bool canRotate)
        {
            character.isPerformingAction = isPerformingAction;
            character.applyRootMotion = applyRootMotion;
            character.canMove = canMove;
            character.canRotate = canRotate;
            character.animator.CrossFade(animationID, .2f);
        }
    }