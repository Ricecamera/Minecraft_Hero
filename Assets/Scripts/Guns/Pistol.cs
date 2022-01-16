using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Shooting
{
    protected override void Awake() {
        currentMagazine = magazineSize;
        base.Awake();
    }

    public override void Shoot() {
        if (IsMagazineEmpty()) {
            if (!isReloading && gameObject) {
                StartCoroutine(Reload());
            }
            return;
        }

        // check if current Time is able to shoot
        if (CanShoot()) {
            AudioManager.instance?.PlaySingle(fireSound);
            Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.Speed = muzzleVelocity;

            // reduce bullets in magazine by one
            currentMagazine--;
            OnShoot?.Invoke(currentMagazine, -1);
            ResetTimer();
        }
    }

    // Pistol have infinite reserveAmmo
    protected override IEnumerator Reload() {

        // Prevent this IEnumerator from called multiple time
        isReloading = true;
        OnReload?.Invoke(reloadTime);
        AudioManager.instance?.PlaySingle(1, reloadSound);
        yield return new WaitForSeconds(reloadTime);
        currentMagazine = magazineSize;
        OnShoot?.Invoke(currentMagazine, -1);
        isReloading = false;
    }
}
