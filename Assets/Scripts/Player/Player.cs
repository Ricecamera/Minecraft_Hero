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
    private bool isInvincible = false;

    public float movementSpeed;
    public float invicibleTime = 2f;

    // Particle effects
    public ParticleSystem deathParticle;

    // Player model and animation
    private Animator playerAnim;
    public GameObject playerAvatar;
    public GameObject invincibleIndicator;

    // Audio
    public AudioClip hurtSound;
    public AudioClip drinkingSound;
    public AudioClip lifeUpSound;
    public AudioClip weaponPickupSound;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        _input = GetComponent<InputHandler>();
        sceneCamera = Camera.main;

        playerAnim = playerAvatar.GetComponent<Animator>();
        StartCoroutine(SetInvincible(invicibleTime));
    }

    // Update is called once per frame
    private void Update()
    {
        bool isGameOver = GameManager.instance.isGameOver;
        bool isGamePause = GameManager.instance.isGamePause;
        if (!(dead | isGameOver | isGamePause)) {
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
                gunController.UpdateCrossHair(hitpoint);
            }

            // Fire a gun
            if (_input.isFire) {
                gunController.Shoot();
            }

            if (_input.Q) {
                gunController.ChangeGun(-1);
                AudioManager.instance.PlaySingle(1, weaponPickupSound);
            }
            else if (_input.E) {
                gunController.ChangeGun(1);
                AudioManager.instance.PlaySingle(1, weaponPickupSound);
            }
        }
    }

    IEnumerator SetInvincible(float invicibleTime) {
        // To Do: trigger invincible effect
        invincibleIndicator.SetActive(true);
        isInvincible = true;
        yield return new WaitForSeconds(invicibleTime);
        isInvincible = false;
        invincibleIndicator.SetActive(false);
    }

    public override void Die(float delay) {
        controller.Reset();
        gunController.DropCurrentGun();
        playerAnim.SetBool("Death_b", true);
        base.Die(delay);
    }

    public override void TakeDamage(float damage) {
        
        if (!isInvincible) {
            AudioManager.instance.PlaySingle(hurtSound);
            health -= damage;
        }
        
        if (health <= 0 && !dead) {
            float dealthDelay = 1.5f;
            Die(dealthDelay);
        }
    }

    public void PickUp(Item item) {
        if (item.CompareTag("HeathPotion")) {
            HeathPotion potion = item as HeathPotion;
            float newHeath = health += potion.HealAmount;
            setHealth(newHeath);
            AudioManager.instance.PlaySingle(2, drinkingSound);
        }
        else if (item.CompareTag("LifeUP")) {
            float newHeath = maxHealth;
            setHealth(newHeath);
            GameManager.instance.IncresePlayerLife(1);
            AudioManager.instance.PlaySingle(2, lifeUpSound);
        }
        else if (item.CompareTag("WeaponDrop")){
            AmmoGun selectedGun = Randomizer.instance.getRandomGun();
            gunController.AddGun(selectedGun);
            AudioManager.instance.PlaySingle(2, weaponPickupSound);
        }
        return;
    }
}
