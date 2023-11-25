using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage Amount")]
    public float physicalDamage = 0f;
    public float magicDamage = 0f;
    public float fireDamage = 0f;
    public float lightningDamage = 0f;
    public float holyDamage = 0f;

    [Header("Final Damage")]
    public int finalDamageDealt = 0;

    [Header("Poise")]
    public float poiseDamage = 0f;
    public bool poiseIsBroken = false;

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSoundFX;

    [Header("Direction Damage Taken From")]
    public float angleHitFrom;
    public Vector3 contactPoint;



    public override void ProcessEffect(CharacterManager characterManager)
    {
        base.ProcessEffect(characterManager);

        if (characterManager.isDead.Value) return;

        CalculateDamage(characterManager);
    }

    private void CalculateDamage(CharacterManager characterManager)
    {
        if (!characterManager.IsOwner) return;

        if (characterCausingDamage != null)
        {

        }

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        Debug.Log("Final Damage Dealt: " + finalDamageDealt);

        characterManager.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
    }
}
