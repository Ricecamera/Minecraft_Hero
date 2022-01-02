using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooting : MonoBehaviour
{

    [SerializeField]
    protected float muzzleVelocity,             // velocity of fired bullets
                    fireRate,                   // fired bullets per second
                    reloadTime;

    [SerializeField]
    protected int currentMagazine, magazineSize;
    protected bool isReloading = false;
    protected float timer;

    protected float msBetweenShots;             // inverse of fire rate
    protected float nextShotTime;               // use to keep track of fire delay time
    protected AudioSource gun_audioSource;

    public Transform firePoint;                 // the position where bullet come out
    public Bullet bulletPrefab;

    public AudioClip fireSound;
    public AudioClip reloadSound;

    protected virtual void Start()
    {
        timer = 0;
        msBetweenShots = 1/fireRate;
        gun_audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update() {
        timer += Time.deltaTime;
    }
    protected abstract IEnumerator Reload();
    public abstract void Shoot();

    public bool CanShoot() {
        Debug.Log(timer);
        return (timer) > msBetweenShots;
    }

    public void ResetTimer() {
        timer = 0;
    }

    public bool IsMagazineEmpty() {
        return currentMagazine == 0;
    }
}
