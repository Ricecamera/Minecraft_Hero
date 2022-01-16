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
    public float attackRange;                   // how far cross hair can go
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
    public UnityEvent<float> OnReload;
    public UnityEvent onDestroyed;

    public bool IsReloading {
        get {
            return isReloading;
        }
    }

    protected virtual void Awake() {
        icon = Resources.Load<Sprite>("Sprites/" + gunTitle);
    }

    protected void Start() {
        isReloading = false;
        msBetweenShots = 1 / fireRate;
        timer = msBetweenShots;
        if (OnShoot == null)
            OnShoot = new UnityEvent<int, int>();
    }

    protected virtual void Update() {
        timer += Time.deltaTime;
    }
    protected abstract IEnumerator Reload();
    public abstract void Shoot();

    public bool CanShoot() {
        return (timer) > msBetweenShots;
    }

    public void ResetTimer() {
        timer = 0;
    }

    public bool IsMagazineEmpty() {
        return currentMagazine == 0;
    }

    private void OnDestroy() {
        onDestroyed?.Invoke();
        onDestroyed.RemoveAllListeners();
        StopCoroutine(Reload());
        AudioManager.instance?.StopSound(1);
    }
}
