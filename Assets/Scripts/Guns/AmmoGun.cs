using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AmmoGun : Shooting {

    public int totalAmmo;
    public int reserveAmmo;

    protected override void Awake() {
        if (totalAmmo >= magazineSize) {
            currentMagazine = magazineSize;
            reserveAmmo = totalAmmo - magazineSize;
        }
        else {
            currentMagazine = totalAmmo;
            reserveAmmo = 0;
        }
        base.Awake();
    }

    protected override IEnumerator Reload() {

        // Prevent this IEnumerator from called multiple time
        isReloading = true;
        OnReload?.Invoke(reloadTime);
        AudioManager.instance?.PlaySingle(1, reloadSound);
        yield return new WaitForSeconds(reloadTime);
        if (reserveAmmo >= magazineSize) {
            currentMagazine = magazineSize;
            reserveAmmo -= magazineSize;
        }
        else {
            currentMagazine = reserveAmmo;
            reserveAmmo = 0;
        }
        OnShoot?.Invoke(currentMagazine, reserveAmmo);
        
        isReloading = false;
    }

    public bool IsBulletEmpty() {
        return currentMagazine == 0 && reserveAmmo == 0;
    }

    public void SetFullAmmo() {
        currentMagazine = magazineSize;
        reserveAmmo = totalAmmo - magazineSize;
        OnShoot?.Invoke(currentMagazine, reserveAmmo);
    }
}
