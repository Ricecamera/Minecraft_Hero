using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity {

    // Controller
    private PlayerController controller;
    private GunController gunController;
    private InputHandler _input;
    private Camera sceneCamera;

    // Player State
    public float movementSpeed;
    //public Transform respawnPos;

    // Particle effects
    public ParticleSystem deathParticle;

    // Player model and animation
    public GameObject playerAvatar;
    private Animator playerAnim;

    // Audio
    public AudioClip hurtSound;
    private AudioSource p_audiosource;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        _input = GetComponent<InputHandler>();
        sceneCamera = Camera.main;

        playerAnim = playerAvatar.GetComponent<Animator>();
        p_audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!dead) {
            // Change 2d input vector to 3d vector;
            Vector3 moveDirection = new Vector3(_input.inputVector.x, 0, _input.inputVector.y);
            Vector3 moveVelocity = moveDirection.normalized * movementSpeed;
            controller.Move(moveVelocity);

            Ray ray = sceneCamera.ScreenPointToRay(_input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            // Find Mouse position and make player look at that position
            if (groundPlane.Raycast(ray, out float rayDistance)) {
                var hitpoint = ray.GetPoint(rayDistance);

                //Debug.DrawRay(ray.origin,ray.direction * 100,Color.red);
                controller.LookAt(hitpoint);
            }

            // Fire a gun
            if (Input.GetButton("Fire1")) {
                gunController.Shoot();
            }
        }
    }
}
