using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bullet
{
    private bool isEnable, isExplosing;
    
    [SerializeField]
    private float exposionRadius, exposionForce;

    public ParticleSystem exposionVfx;

    protected new void Start() {
        Speed = 0;
        isEnable = false;
        isExplosing = false;
    }
    protected override void CheckCollisions(float moveDistance) {
        if (isEnable) {
            Ray ray = new Ray(transform.position, transform.forward);

            Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .2f, collisionMask);
            if (Physics.Raycast(ray, out RaycastHit hit, moveDistance + skinWidth, collisionMask) && !isExplosing ||
                initialCollisions.Length > 0) {
                isExplosing = true;
                Instantiate(exposionVfx, transform.position, Quaternion.identity);
                this.Speed = 0.2f;
                Collider[] collisions = Physics.OverlapSphere(transform.position, exposionRadius, collisionMask);
                for (int i = 0; i < collisions.Length; i++) {
                    OnHitObject(collisions[i]);
                }

                GameObject.Destroy(gameObject);
            }
        }
    }

    protected override void OnHitObject(Collider c) {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        Rigidbody objectRb = c.GetComponent<Rigidbody>();
        if (damageableObject != null) {
            damageableObject.TakeDamage(damage);
            Vector3 forceDir = (objectRb.transform.position - transform.position).normalized;
            objectRb.AddForce(exposionForce * forceDir, ForceMode.Impulse);
            
        }
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, exposionRadius);
    }

    public void Launch(float muzzleVelocity) {
        isEnable = true;
        Speed = muzzleVelocity;
        transform.SetParent(null, true);
    }
}