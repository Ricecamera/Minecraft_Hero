using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private Shooting equippedGun;

    public Transform weaponHold;
    public Shooting startingGun;

    void Start()
    {
        if (startingGun != null) {
            EquipGun(startingGun);
        }
    }

    public void EquipGun(Shooting gunToEquip) {
        if (equippedGun != null) {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Shooting;
        equippedGun.transform.parent = weaponHold;
    }

    public void Shoot() {
        if (equippedGun != null) {
            equippedGun.Shoot();
        }
    }
}
