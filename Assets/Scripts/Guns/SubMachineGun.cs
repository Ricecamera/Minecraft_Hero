using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMachineGun : AmmoGun {
    // Start is called before the first frame update

    public override void Shoot() {

        if (IsMagazineEmpty() && gameObject) {
            if (!IsBulletEmpty() && !isReloading) {
                StartCoroutine(Reload());
            }
            return;
        }
        // check if current Time is able to shoot
        if (CanShoot() && !IsMagazineEmpty()) {
            Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.Speed = muzzleVelocity;

            // reduce bullets in magazine by one
            currentMagazine--;
            OnShoot?.Invoke(currentMagazine, reserveAmmo);
            AudioManager.instance.PlaySingle(fireSound);
            ResetTimer();
        }


    }
}
