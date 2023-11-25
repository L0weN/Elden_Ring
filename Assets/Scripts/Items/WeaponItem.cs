using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Requirements")]
    public int strengthREQ;
    public int dexREQ;
    public int intREQ;
    public int faithREQ;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int magicDamage = 0;
    public int fireDamage = 0;
    public int lightningDamage = 0;
    public int holyDamage = 0;

    [Header("Weapon Base Poise Damage")]
    public float poiseDamage = 10;

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;

    [Header("Actions")]
    public WeaponItemAction oh_RB_Action;
}
