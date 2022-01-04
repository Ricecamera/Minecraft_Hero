using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Pickup : MonoBehaviour
{   
    [SerializeField]
    private float destroyDelay = 5f;

    private void Start() {
        Invoke("DestroySelf", destroyDelay);
    }

    protected void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            ONPickup(player);
            CancelInvoke("DestroySelf");
            Invoke("DestroySelf", 0f);
        }
    }

    public abstract void ONPickup(Player player);

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
