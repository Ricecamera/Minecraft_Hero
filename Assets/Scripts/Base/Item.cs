using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    [SerializeField]
    private float angularVelocity = 50f;

    [SerializeField]
    private float destroyDelay = 15f;


    private void Start() {
        Invoke("DestroySelf", destroyDelay);
    }

    void Update() {
        bool isGameOver = GameManager.instance.isGameOver;
        if (isGameOver) {
            Destroy(gameObject);
        }

        transform.Rotate(0, angularVelocity * Time.deltaTime, 0);
    }

    protected void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            ONPickup(other);
        }
    }

    public virtual void ONPickup(Collider other) {
        Player player = other.GetComponent<Player>();
        if (player != null)
            player.PickUp(this);
        CancelInvoke("DestroySelf");
        Invoke("DestroySelf", 0f);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
