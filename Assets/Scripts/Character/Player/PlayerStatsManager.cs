using PLAYER;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager playerManager;
    protected override void Awake()
    {
        base.Awake();

        playerManager = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        CalculateHealthBasedOnVitalityLevel(playerManager.playerNetworkManager.vitality.Value);
        CalculateStaminaBasedOnEnduranceLevel(playerManager.playerNetworkManager.endurance.Value);
    }
}
