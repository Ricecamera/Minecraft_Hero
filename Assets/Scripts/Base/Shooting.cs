using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Shooting : MonoBehaviour
{
    protected bool isReloading = false;
    protected float timer;

    protected float msBetweenShots;             // inverse of fire rate
    protected float nextShotTime;               // use to keep track of fire delay time

    [Header("Identifier")]
    public int gunId;
    public string gunTitle;

    [Header("Stats")]
    public float muzzleVelocity;                // velocity of fired bullets
    public float fireRate;                      // fired bullets per second
    public float reloadTime;
    public int currentMagazine, magazineSize;
    
    [Header("References")]
    public Transform firePoint;                 // the position where bullet come out
    public Bullet bulletPrefab;
    public Sprite icon;

    [Header("Sound effects")]
    public AudioClip fireSound;
    public AudioClip reloadSound;

    public UnityEvent<int, int> OnShoot;

    private void Awake() {
        icon = Resources.Load<Sprite>("Sprites/" + gunTitle);
    }
    protected virtual void Start()
    {
        timer = 0;
        msBetweenShots = 1/fireRate;
        if (OnShoot == null)
            OnShoot = new UnityEvent<int, int>();
    }

    protected virtual void Update() {
        timer += Time.deltaTime;
    }
    protected abstract IEnumerator Reload();
    public abstract void Shoot();

    public bool CanShoot() {
        //Debug.Log(timer);
        return (timer) > msBetweenShots;
    }

    public void ResetTimer() {
        timer = 0;
    }

    public bool IsMagazineEmpty() {
        return currentMagazine == 0;
    }
}
