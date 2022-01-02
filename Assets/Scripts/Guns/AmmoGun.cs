using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AmmoGun : Shooting {

    public int reserveAmmo;

    [SerializeField]
    protected int totalAmmo;

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
                gun_audioSource.PlayOneShot(reloadSound, .5f);
                StartCoroutine(Reload());
            }
            return;
        }

        // check if current Time is able to shoot
        if (Time.time > nextShotTime) {
            nextShotTime = Time.time + msBetweenShots / 1000;
            gun_audioSource.PlayOneShot(fireSound);
            Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.Speed = muzzleVelocity;

            // reduce bullets in magazine by one
            currentMagazine--;
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
