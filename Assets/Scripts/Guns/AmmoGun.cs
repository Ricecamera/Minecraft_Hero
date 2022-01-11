using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AmmoGun : Shooting {

    [SerializeField]
    protected int totalAmmo;

    public int reserveAmmo;

    protected override void Start() {
        if (totalAmmo >= magazineSize) {
            currentMagazine = magazineSize;
            reserveAmmo = totalAmmo - magazineSize;
        }
        else {
            currentMagazine = totalAmmo; ;
            reserveAmmo = 0;
        }
        base.Start();
    }

    protected override IEnumerator Reload() {

        // Prevent this IEnumerator from called multiple time
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);
        if (reserveAmmo >= magazineSize) {
            currentMagazine = magazineSize;
            reserveAmmo -= magazineSize;
        }
        else {
            currentMagazine = reserveAmmo;
            reserveAmmo = 0;
        }
        isReloading = false;
    }

    public bool IsBulletEmpty() {
        return currentMagazine == 0 && reserveAmmo == 0;
    }
}
