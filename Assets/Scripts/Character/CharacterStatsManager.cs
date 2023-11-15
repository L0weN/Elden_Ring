using CHARACTER;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager characterManager;

    [Header("Stamina Regeneration")]
    [SerializeField] float staminaRegenAmount = 2f;
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2f;

    protected virtual void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }
    public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        float stamina = 0;

        stamina = endurance * 10;
        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina()
    {
        if (!characterManager.IsOwner) return;

        if (characterManager.characterNetworkManager.isSprinting.Value) return;

        if (characterManager.isPerformingAction) return;

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (characterManager.characterNetworkManager.currentStamina.Value < characterManager.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= .1)
                {
                    staminaTickTimer = 0;
                    characterManager.characterNetworkManager.currentStamina.Value += staminaRegenAmount;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenerationTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        if (currentStaminaAmount < previousStaminaAmount)
        {
            staminaRegenerationTimer = 0;
        }
    }
}
