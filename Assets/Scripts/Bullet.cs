using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float xBound = 26.6f;
    private float zBound = 15.0f;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        else if (collision.gameObject.CompareTag("Zombie"))
        {
            Zombie zombie = collision.gameObject.GetComponent<Zombie>();
            zombie.TakeDamage(1);
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Vector3 curPos = gameObject.transform.position;
        if (Mathf.Abs(curPos.x) > xBound || Mathf.Abs(curPos.z) > zBound)
        {
            Destroy(gameObject);
        }
    }
}
