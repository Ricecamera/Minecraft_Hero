using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public PlayerController player;
    private GameManager gameManager;

    private AudioSource gun_audioSource;
    public AudioClip fireSound;

    public float bulletForce = 20.0f;
    public float gunDelay = 0.5f;
    private bool fireAble = true;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gun_audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (gameManager.isGameOver) return;

        if (Input.GetButtonDown("Fire1") && fireAble)
        {
            Shoot();
            StartCoroutine(FireDelay());
        }
    }

    private void Shoot()
    {
        gun_audioSource.PlayOneShot(fireSound);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode.Impulse);
    }

    IEnumerator FireDelay()
    {
        fireAble = false;
        yield return new WaitForSeconds(gunDelay);
        fireAble = true;
    }
}
