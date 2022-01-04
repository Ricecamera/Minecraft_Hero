using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AmmoGun : Shooting {

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

    public override void Shoot() {
        if (IsMagazineEmpty()) {
            if (!IsBulletEmpty() && !isReloading) {
                AudioManager.instance.PlaySingle(reloadSound);
                StartCoroutine(Reload());
            }
            return;
        }

        // check if current Time is able to shoot
        if (CanShoot()) {
            AudioManager.instance.PlaySingle(fireSound);
            Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.Speed = muzzleVelocity;

            // reduce bullets in magazine by one
            currentMagazine--;
            ResetTimer();
        }
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
