using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public int currentGun = 0;
    public int selectIdx = 0;
    private int equippedGunIdx;
    private readonly int maxGun = 3;

    public Animator playerAnim;
    public Transform[] weaponHolds;
    public List<Shooting> weaponInventory;

    public Shooting startingGun;

    void Start()
    {
        if (startingGun != null) {
            AddGun(startingGun);
            equippedGunIdx = selectIdx = 0;
            EquipGun(0);
        }
    }



    private int hasDuplicateGun(System.Type gunType) {

        for (int i = 0; i < currentGun; i++) {
            if (gunType == weaponInventory[i].GetType()) {
                return i;
            }
        }
        return -1;
    }

    private void EquipGun(int index) {
        for (int i = 0; i < weaponInventory.Count; i++) {
            if (weaponInventory[i] != null) {
                if (i == index)
                    weaponInventory[i].gameObject.SetActive(true);
                else
                    weaponInventory[i].gameObject.SetActive(false);
            }
        }

        equippedGunIdx = index;
        var gunType = weaponInventory[index].GetType();
        if (gunType == typeof(SubMachineGun)) {
            playerAnim.SetInteger("WeaponType_int", 2);
        }
        else if (gunType == typeof(RocketLauncher)) {
            playerAnim.SetInteger("WeaponType_int", 8);
        }
        else if (gunType == typeof(Shotgun)){
            playerAnim.SetInteger("WeaponType_int", 4);
        }
        else {
            playerAnim.SetInteger("WeaponType_int", 1);
        }
    }

    private void DropCurrentGun(int toDropIdx) {
        // Destroy gameObject
        Shooting equippedGun = weaponInventory[equippedGunIdx];
        Destroy(equippedGun.gameObject);

        // Shift left
        for (int i = toDropIdx; i < currentGun - 1; i++) {
            weaponInventory[i] = weaponInventory[i+1];
        }
        // Set last gun in inventory to null to prevent duplicated data
        weaponInventory[currentGun - 1] = null;
        currentGun--;

        // Set current gun to pistol
        equippedGunIdx = 0;
        EquipGun(0);
    }

    public void Shoot() {
        Shooting equippedGun = weaponInventory[equippedGunIdx];
        if (equippedGun != null) {
            equippedGun.Shoot();
            AmmoGun ammoGun = equippedGun as AmmoGun;
            if (ammoGun != null && ammoGun.IsBulletEmpty()) {
                DropCurrentGun(equippedGunIdx);
            }
        }
    }

    public bool AddGun(Shooting gunToAdd) {
        Transform weaponHold;
        var gunType = gunToAdd.GetType();
        if (gunType == typeof(Pistol)) {
            weaponHold = weaponHolds[0];
        }
        else if (gunType == typeof(RocketLauncher)) {
            weaponHold = weaponHolds[2];
        }
        else {
            weaponHold = weaponHolds[1];
        }


        if (currentGun < maxGun) {
            Shooting equippedGun = Instantiate(gunToAdd, weaponHold.position, weaponHold.rotation) as Shooting;
            equippedGun.transform.parent = weaponHold;
            equippedGun.gameObject.SetActive(false);
            
            int FoundIdx = hasDuplicateGun(gunType);
            if (FoundIdx != -1) {
                Destroy(weaponInventory[FoundIdx].gameObject);
                weaponInventory[FoundIdx] = equippedGun;
            }
            else {
                weaponInventory[currentGun] = equippedGun;
                currentGun++;
            }
            return true;
        }
        return false;
    }

    public void ChangeGun(int direction) {
        int newIdx = selectIdx + direction;
        if (newIdx >= maxGun)
            newIdx = 0;
        else if (newIdx < 0)
            newIdx = maxGun - 1;

        selectIdx = newIdx;
        if (weaponInventory[selectIdx] != null)
            EquipGun(selectIdx);
    }
}
