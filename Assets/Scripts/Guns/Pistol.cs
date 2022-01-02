using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Shooting
{
    protected override void Start() {
        currentMagazine = magazineSize;
        base.Start();
    }

    public override void Shoot() {
        if (IsMagazineEmpty()) {
            if (!isReloading) {
                gun_audioSource.PlayOneShot(reloadSound, .5f);
                StartCoroutine(Reload());
            }
            return;
        }

        // check if current Time is able to shoot
        if (CanShoot()) {
            gun_audioSource.PlayOneShot(fireSound);
            Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.Speed = muzzleVelocity;

            // reduce bullets in magazine by one
            currentMagazine--;
            ResetTimer();
        }
    }

    // Pistol have infinite reserveAmmo
    protected override IEnumerator Reload() {

        // Prevent this IEnumerator from called multiple time
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentMagazine = magazineSize;
        isReloading = false;
    }
}
