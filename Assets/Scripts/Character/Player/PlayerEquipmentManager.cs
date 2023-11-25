using PLAYER;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager playerManager;

    public WeaponModelInstantionSlot rightHandSlot;
    public WeaponModelInstantionSlot leftHandSlot;

    [SerializeField] WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();

        playerManager = GetComponent<PlayerManager>();

        InitializeWeaponSlots();
    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponsOnBothHands();
    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantionSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantionSlot>();

        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    public void SwitchRightWeapon()
    {
        if (!playerManager.IsOwner) return;

        playerManager.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon", false, true, true, true);

        WeaponItem selectedWeapon = null;

        playerManager.playerInventoryManager.rightHandWeaponIndex += 1;

        if(playerManager.playerInventoryManager.rightHandWeaponIndex < 0 || playerManager.playerInventoryManager.rightHandWeaponIndex > 2)
        {
            playerManager.playerInventoryManager.rightHandWeaponIndex = 0;

            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosition = 0;

            for (int i = 0; i < playerManager.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
            {
                if (playerManager.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    weaponCount += 1;

                    if (firstWeapon == null)
                    {
                        firstWeapon = playerManager.playerInventoryManager.weaponsInRightHandSlots[i];
                        firstWeaponPosition = i;
                    }
                }
            }

            if (weaponCount <= 1)
            {
                playerManager.playerInventoryManager.rightHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                playerManager.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
            }
            else
            {
                playerManager.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                playerManager.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
            }

            return;
        }

        foreach(WeaponItem weapon in playerManager.playerInventoryManager.weaponsInRightHandSlots)
        {
            if (playerManager.playerInventoryManager.weaponsInRightHandSlots[playerManager.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                selectedWeapon = playerManager.playerInventoryManager.weaponsInRightHandSlots[playerManager.playerInventoryManager.rightHandWeaponIndex];

                playerManager.playerNetworkManager.currentRightHandWeaponID.Value = playerManager.playerInventoryManager.weaponsInRightHandSlots[playerManager.playerInventoryManager.rightHandWeaponIndex].itemID;
                return;
            }
        }

        if (selectedWeapon == null && playerManager.playerInventoryManager.rightHandWeaponIndex <= 2)
        {
            SwitchRightWeapon();
        }
        else
        {
            
        }
    }
    public void LoadRightWeapon()
    {
        if (playerManager.playerInventoryManager.currentRightHandWeapon != null)
        {
            rightHandSlot.UnloadWeapon();
            rightHandWeaponModel = Instantiate(playerManager.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(playerManager, playerManager.playerInventoryManager.currentRightHandWeapon);
        }
    }

    public void SwitchLeftWeapon()
    {

    }

    public void LoadLeftWeapon()
    {
        leftHandSlot.UnloadWeapon();
        if (playerManager.playerInventoryManager.currentLeftHandWeapon != null)
        {
            leftHandWeaponModel = Instantiate(playerManager.playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel);
            leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(playerManager, playerManager.playerInventoryManager.currentLeftHandWeapon);
        }
    }
}
