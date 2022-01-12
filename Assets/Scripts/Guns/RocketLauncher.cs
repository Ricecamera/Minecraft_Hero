using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : AmmoGun
{
    Rocket currentRocket = null;

    protected override void Awake() {
        if (totalAmmo >= magazineSize) {
            currentMagazine = magazineSize;
            reserveAmmo = totalAmmo - magazineSize;
        }
        else {
            currentMagazine = totalAmmo;
            reserveAmmo = 0;
        }
        
        if (currentMagazine >= 0)
            currentRocket = Instantiate(bulletPrefab, firePoint) as Rocket;
        base.Awake();
    }

    public override void Shoot() {
        // check if current Time is able to shoot
        if (CanShoot() && !IsMagazineEmpty()) {
            currentRocket.Launch(muzzleVelocity);

            // reduce bullets in magazine by one
            currentMagazine--;
            OnShoot?.Invoke(currentMagazine, reserveAmmo);
            ResetTimer();
        }

        if (IsMagazineEmpty()) {
            if (!IsBulletEmpty() && !isReloading) {
                StartCoroutine(Reload());
            }
            return;
        }
    }

    protected override IEnumerator Reload() {

        // Prevent this IEnumerator from called multiple time
        yield return base.Reload();

        if (currentMagazine >= 0)
            currentRocket = Instantiate(bulletPrefab, firePoint) as Rocket;
    }
}
