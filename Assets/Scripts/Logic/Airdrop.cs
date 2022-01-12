using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Airdrop : MonoBehaviour, IDamageable {

    [SerializeField]
    private float durability = 3;

    private float damage = 2;
    private bool destroyed = false;
    private Rigidbody boxRb;

    public float itemYOffset = 2.2f;
    public float destroyDelay = .25f;
    public float dropDamageRadius = 3f;
    public float pushForce = 5f;
    

    public Item itemToSpawn = null;
    public LayerMask collisionMask;
    public ParticleSystem smokeVfx;
    public UnityEvent OnDestroy;


    void Start() {
        itemToSpawn = Randomizer.instance.getRandomItem();
        boxRb = GetComponent<Rigidbody>();
        boxRb.isKinematic = false;
    }

    void Update() {
        bool isGameOver = GameManager.instance.isGameOver;
        if (isGameOver) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Ground")) {
            boxRb.isKinematic = true;
            Collider[] collisions = Physics.OverlapSphere(transform.position, dropDamageRadius, collisionMask);
            if (collisions.Length > 0) {
                for (int i = 0; i < collisions.Length; i++) {
                    OnHitObject(collisions[i]);
                }
            }
        }
    }

    private void OnHitObject(Collider c) {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null) {
            Rigidbody objectRb = c.GetComponent<Rigidbody>();
            Vector3 forceDir = (c.transform.position - transform.position).normalized;
            objectRb.AddForce(forceDir * pushForce, ForceMode.Impulse);
            damageableObject.TakeDamage(damage);
        }
    }

    private void SpawnItem() {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + itemYOffset, transform.position.z);
        Item dropItem = Instantiate(itemToSpawn, spawnPos, Quaternion.identity);
    }

    public void TakeHit(float damage, RaycastHit hit) {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage) {
        durability -= damage;
        if (durability <= 0 && !destroyed) {
            destroyed = true;
            SpawnItem();
            Invoke("Destroy", destroyDelay);
        }
    }

    public void Destroy() {
        Instantiate(smokeVfx, transform.position, Quaternion.identity);
        OnDestroy?.Invoke();
        OnDestroy.RemoveAllListeners();
        GameObject.Destroy(gameObject);
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dropDamageRadius);
    }
}
