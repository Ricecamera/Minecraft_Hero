using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    protected float speed;
    public float Speed {
        get { return speed; }
        set {
            if (value < 0)
                speed = 0;
            else
                speed = value;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            return;
        }
        else if (collision.gameObject.CompareTag("Zombie")) {
            Zombie zombie = collision.gameObject.GetComponent<Zombie>();
            zombie.TakeDamage(1);
        }
        Destroy(gameObject);
    }

    protected virtual void FixedUpdate() {
        float xBound = CameraManager.instance.Bound.x;
        float zBound = CameraManager.instance.Bound.z;

        transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
        if (Mathf.Abs(transform.position.x) > xBound + 2 || Mathf.Abs(transform.position.z) > zBound + 2) {
            Destroy(gameObject);
        }
    }
}
