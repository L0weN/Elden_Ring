using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace CHARACTER
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager characterManager;
        
        /*[Header("Network Position")]
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;*/
        

        [Header("Animator")]
        public NetworkVariable<float> animatorHorizontalParameter = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> animatorVerticalParameter = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> networkMoveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Stats")]
        public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
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
            characterManager.isPerformingAction = isPerformingAction;
            characterManager.applyRootMotion = applyRootMotion;
            characterManager.canMove = canMove;
            characterManager.canRotate = canRotate;
            characterManager.animator.CrossFade(animationID, .2f);
        }
    }
}