using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooting : MonoBehaviour
{

    public Transform firePoint;                 // the position where bullet come out
    public Bullet bulletPrefab;

    public AudioClip fireSound;
    public AudioClip reloadSound;

    public int currentMagazine;
    protected bool isReloading = false;

    [SerializeField]
    protected float muzzleVelocity,             // velocity of fired bullets
                    fireRate,                   // fired bullets per second
                    reloadTime;

    [SerializeField]
    protected int magazineSize;
   

    protected float msBetweenShots;             // inverse of fire rate
    protected float nextShotTime;               // use to keep track of fire delay time
    protected AudioSource gun_audioSource;


    //public int CurrentMagazine { get; }
    //public int CurrentBullet { get; }

    protected virtual void Start()
    {
        msBetweenShots = 1000/fireRate;
        gun_audioSource = GetComponent<AudioSource>();
    }

    public abstract void Shoot();

    protected abstract IEnumerator Reload();

    public bool IsMagazineEmpty() {
        return currentMagazine == 0;
    }
}
