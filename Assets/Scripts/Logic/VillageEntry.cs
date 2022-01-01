using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageEntry : MonoBehaviour
{
    GameManager gameManager;
    private AudioSource damageSound;
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        damageSound = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            damageSound.Play();
            gameManager.lifePoint--;
            Destroy(other.gameObject);

            if (gameManager.lifePoint <= 0)
            {
                gameManager.isGameOver = true;
            }
        }
    }
}
