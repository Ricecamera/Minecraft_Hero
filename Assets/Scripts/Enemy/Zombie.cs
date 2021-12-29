using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    
    private GameObject player;
    private Player playeScript;
    private GameManager gameManager;
    public LayerMask whatIsGround, whatIsPlayer;
    public ParticleSystem blood;

    public int health;
    public int speed;

    // Sound effect
    private AudioSource zombie_audioSource;
    public AudioClip moanSound;

    //Patroling
    Transform goal;

    //Attacking
    public float timeBetweenAttacks;
    public float pushFroce;
    public int atk;
    bool alreadyAttacked;
    
    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    void Awake()
    {
        goal = GameObject.Find("Village Entry").transform;
        player = GameObject.Find("Player");
        playeScript = player.GetComponent<Player>();
        zombie_audioSource = GetComponent<AudioSource>();
        zombie_audioSource.PlayOneShot(moanSound);
        //agent = GetComponent<NavMeshAgent>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameOver) return;
        //Check for sight and attack range
        playerInSightRange = (playeScript.isDead == false) && Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = (playeScript.isDead == false) && Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        Vector3 target = goal.position;
        target.y = transform.position.y;
        Vector3 distanceToWalkPoint = (target - transform.position);
        Vector3 direction = distanceToWalkPoint.normalized;

        transform.LookAt(target);
        transform.position += direction * speed * Time.deltaTime;
    }

    private void ChasePlayer()
    {
        Vector3 target = player.transform.position;
        target.y = transform.position.y;
        Vector3 distanceToWalkPoint = (target - transform.position);
        Vector3 direction = distanceToWalkPoint.normalized;

        transform.LookAt(target);
        transform.position += direction * speed * Time.deltaTime;
    }

    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
            //Attack code here
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            playerRb.AddForce(transform.forward * pushFroce, ForceMode.Impulse);
            playeScript.TakeDamge(atk);
            //End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int Atk)
    {
        health -= Atk;
        blood.Play();
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.2f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
