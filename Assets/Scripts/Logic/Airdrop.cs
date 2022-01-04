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

    public float destroyDelay = .5f;
    public float dropDamageRadius = 3f;
    public float pushForce = 5f;

    public Pickup itemToSpawn = null;
    public LayerMask collisionMask;
    public UnityEvent OnDestroy;


    void Start() {
        boxRb = GetComponent<Rigidbody>();
        boxRb.isKinematic = false;
    }

    void FixedUpdate() {
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
        //Instantiate(itemToSpawn, transform);
    }

    public void TakeHit(float damage, RaycastHit hit) {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage) {
        durability -= damage;
        if (durability <= 0 && !destroyed) {
            destroyed = true;
            SpawnItem();
            Destroy(gameObject, destroyDelay);
        }
        print("Box taken damage " + damage);
    }

    public void Destroy() {
        OnDestroy?.Invoke();
        GameObject.Destroy(gameObject, destroyDelay);
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dropDamageRadius);
    }
}
