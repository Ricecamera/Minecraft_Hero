using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player State
    public int health = 3;
    public float movementSpeed;
    public bool isDead;

    // controller
    private PlayerController controller;
    private InputHandler _input;
    //private GameManager gameManager;
  
    private Camera sceneCamera;


    //Player model and animation
    public GameObject playerAvatar;
    public ParticleSystem deathParticle;
    private Animator playerAnim;
    

    //public Transform respawnPos;

    // Audio
    private AudioSource p_audiosource;
    public AudioClip hurtSound;



    // Start is called before the first frame update
    private void Start()
    {
        // get a reference of gameManger to check games state
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        controller = GetComponent<PlayerController>();
        _input = GetComponent<InputHandler>();
        sceneCamera = Camera.main;

        playerAnim = playerAvatar.GetComponent<Animator>();
        p_audiosource = GetComponent<AudioSource>();

        isDead = false;
    }

    // Update is called once per frame
    private void Update()
    {
        //if (gameManager.isGameOver)
        //{
        //    controller.Reset();
        //    return;
        //}

        // Change 2d input vector to 3d vector;
        if (!isDead)
        {
            var targetVector = new Vector3(_input.inputVector.x, 0, _input.inputVector.y);
            var moveVelocity = targetVector.normalized * movementSpeed;
            controller.Move(moveVelocity);

            Ray ray = sceneCamera.ScreenPointToRay(_input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            // Find Mouse position and make player look at that position
            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                var hitpoint = ray.GetPoint(rayDistance);

                //Debug.DrawRay(ray.origin,ray.direction * 100,Color.red);
                controller.LookAt(hitpoint);

            }
        }

    }

    public void TakeDamge(int damage)
    {
        p_audiosource.PlayOneShot(hurtSound);
        health -= damage;
        if (health <= 0)
        {
            controller.Reset();
            OnDeathEntry();
        }
    }


    private void OnDeathEntry()
    {
        isDead = true;
        deathParticle.Play();
        playerAnim.SetBool("Death_b", true);
        //StartCoroutine(Respawn());

    }

    //ienumerator respawn()
    //{
    //    yield return new waitforseconds(2.2f);
    //    health = 3;
    //    playerrb.rotation = quaternion.identity;
    //    transform.position = respawnpos.position;
    //    playeranim.setbool("death_b", false);
    //    isdead = false;
    //}
}
