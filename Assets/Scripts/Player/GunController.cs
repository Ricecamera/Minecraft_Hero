using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public static readonly int MAX_GUN = 3;

    private int equippedGunIdx;
    private int currentGuns = 0;
    private int selectIdx = 0;

    private UIManager UImanager;

    

    public Animator playerAnim;
    public Transform[] weaponHolds;
    public List<Shooting> weaponInventory;

    public Shooting startingGun;
    public Transform crossHair;

    void Start()
    {
        UImanager = GameObject.Find("UI").GetComponent<UIManager>();
        crossHair = GameObject.Find("Cross hair").GetComponent<Transform>();
        if (startingGun != null) {
            AddGun(startingGun);
            equippedGunIdx = selectIdx = 0;
            EquipGun(0);
            UImanager.UpdateGun(weaponInventory[0], 1);
        }
    }

    public void UpdateCrossHair(Vector3 target) {
        Shooting equippedGun = weaponInventory[equippedGunIdx];
        target.y = equippedGun.transform.position.y;
        

        if (equippedGun != null) {
            float distance = (target - equippedGun.transform.position).magnitude * 0.9f;
            Vector3 newCrossHairPos = equippedGun.transform.position + equippedGun.transform.forward * distance;
            crossHair.position = newCrossHairPos;
        }
    }

    private int HasDuplicateGun(int gunId) {

        for (int i = 0; i < currentGuns; i++) {
            if (gunId == weaponInventory[i].gunId) {
                return i;
            }
        }
        return -1;
    }

    private void EquipGun(int index) {
        weaponInventory[index].OnShoot.RemoveListener(UImanager.UpdateAmmo);
        for (int i = 0; i < weaponInventory.Count; i++) {
            if (weaponInventory[i] != null) {
                if (i == index)
                    weaponInventory[i].gameObject.SetActive(true);
                else
                    weaponInventory[i].gameObject.SetActive(false);
            }
        }
        weaponInventory[index].OnShoot.AddListener(UImanager.UpdateAmmo);

        equippedGunIdx = index;
        var gunId = weaponInventory[index].gunId;
        if (gunId == 1) {
            playerAnim.SetInteger("WeaponType_int", 2);
        }
        else if (gunId == 2) {
            playerAnim.SetInteger("WeaponType_int", 4);
        }
        else if (gunId == 3){
            playerAnim.SetInteger("WeaponType_int", 8);
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
        for (int i = toDropIdx; i < currentGuns - 1; i++) {
            weaponInventory[i] = weaponInventory[i+1];
        }
        // Set last gun in inventory to null to prevent duplicated data
        weaponInventory[currentGuns - 1] = null;
        currentGuns--;

        // Set current gun to pistol
        equippedGunIdx = selectIdx = 0;
        UImanager.UpdateGun(weaponInventory[0], 1);
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
        var gunId = gunToAdd.gunId;
        if (gunId == 0) {
            weaponHold = weaponHolds[0];
        }
        else if (gunId == 3) {
            weaponHold = weaponHolds[2];
        }
        else {
            weaponHold = weaponHolds[1];
        }


        if (currentGuns < MAX_GUN) {
            Shooting equippedGun = Instantiate(gunToAdd, weaponHold.position, weaponHold.rotation) as Shooting;
            equippedGun.transform.parent = weaponHold;
            equippedGun.gameObject.SetActive(false);
            
            int foundIdx = HasDuplicateGun(gunId);
            if (foundIdx != -1) {

                if (weaponInventory[foundIdx] is AmmoGun) {
                    AmmoGun temp = weaponInventory[foundIdx] as AmmoGun;
                    temp.SetFullAmmo();
                }
            }
            else {
                weaponInventory[currentGuns] = equippedGun;
                currentGuns++;
            }

            if (selectIdx == (currentGuns - 1) || (selectIdx != -1 && selectIdx == foundIdx)) {
                UImanager.UpdateGun(weaponInventory[selectIdx], selectIdx + 1);
                EquipGun(selectIdx);
            }
                
            return true;
        }
        return false;
    }

    public void ChangeGun(int direction) {
        if (weaponInventory[equippedGunIdx].IsReloading) return;

        int newIdx = selectIdx + direction;
        if (newIdx >= MAX_GUN)
            newIdx = 0;
        else if (newIdx < 0)
            newIdx = MAX_GUN - 1;

        selectIdx = newIdx;
        UImanager.UpdateGun(weaponInventory[selectIdx], selectIdx + 1);
        if (weaponInventory[selectIdx] != null)
            EquipGun(selectIdx);
    }
}
