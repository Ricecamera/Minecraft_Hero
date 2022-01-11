using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    protected float speed = 10, damage = 1;

    protected float lifetime = 10;
    protected float skinWidth = .5f;

    public LayerMask collisionMask;
    public float Speed {
        get { return speed; }
        set {
            if (value < 0)
                speed = 0;
            else
                speed = value;
        }
    }
    protected void Start() {
        Destroy(gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initialCollisions.Length > 0) {
            OnHitObject(initialCollisions[0]);
        }
    }

    protected void FixedUpdate() {
        float xBound = CameraManager.instance.Bound.x;
        float zBound = CameraManager.instance.Bound.z;

        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
        if (Mathf.Abs(transform.position.x) > xBound + 2 || Mathf.Abs(transform.position.z) > zBound + 2) {
            Destroy(gameObject);
        }
    }

    protected virtual void CheckCollisions(float moveDistance) {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) {
            OnHitObject(hit);
        }
    }

    protected virtual void OnHitObject(RaycastHit hit) {
        print(hit.collider.gameObject.name);
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null) {
            damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }

    protected virtual void OnHitObject(Collider c) {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null) {
            damageableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject);
    }
}
