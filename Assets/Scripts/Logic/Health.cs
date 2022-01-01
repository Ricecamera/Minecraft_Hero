using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    Player player;
    public const int fullHealth = 3;
    public RawImage[] hearts;

    public void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    public void Update()
    {
        for (int i = 0 ; i < hearts.Length; ++i)
        {
            if (i < player.health)
            {
                hearts[i].enabled = true;
            } else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
