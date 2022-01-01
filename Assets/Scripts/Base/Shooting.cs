using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooting : MonoBehaviour
{
    protected bool isReloading = false;

    [SerializeField]
    protected float muzzleVelocity,             // velocity of fired bullets
                    fireRate,                   // fired bullets per second
                    reloadTime;

    [SerializeField]
    protected int magazineSize, totalBullets;
    public int currentMagazine, reserveBullets;

    protected float msBetweenShots;             // inverse of fire rate
    protected float nextShotTime;               // use to keep track of fire delay time
    protected AudioSource gun_audioSource;

    public Transform firePoint;                 // the position where bullet come out
    public Bullet bulletPrefab;

    public AudioClip fireSound;
    public AudioClip reloadSound;

    //public int CurrentMagazine { get; }
    //public int CurrentBullet { get; }

    protected virtual void Start()
    {
        if (totalBullets >= magazineSize) {
            currentMagazine = magazineSize;
            reserveBullets = totalBullets - magazineSize;
        }
        else {
            currentMagazine = totalBullets;
            reserveBullets = 0;
        }
        msBetweenShots = 1000/fireRate;
        gun_audioSource = GetComponent<AudioSource>();
    }

    public abstract void Shoot();

    protected IEnumerator Reload() {
        // Todo: implement magazine and reload machanic

        // Prevent this IEnumerator from called multiple time
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);
        if (reserveBullets >= magazineSize) {
            currentMagazine = magazineSize;
            reserveBullets -= magazineSize;
        }
        else {
            currentMagazine = reserveBullets;
            reserveBullets = 0;
        }
        isReloading = false;
    }

    public bool IsMagazineEmpty() {
        return currentMagazine == 0;
    }

    public bool IsBulletEmpty() {
        return currentMagazine == 0 && reserveBullets == 0;
    }

}
