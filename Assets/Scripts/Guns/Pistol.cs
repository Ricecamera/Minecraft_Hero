using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Shooting
{
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
}
