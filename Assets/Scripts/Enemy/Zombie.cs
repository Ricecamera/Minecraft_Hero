using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : LivingEntity {
    public enum State { Patrolling, Chasing, Attacking };

    private static bool hasTarget;
    private State currentState = State.Patrolling;
    private float nextAttackTime;
    private float myCollisionRadius;
    private float targetCollisionRadius;
    private float timer;

    private NavMeshAgent agent;
    private Transform target;
    private LivingEntity targetEntity;


    public LayerMask whatIsGround;
    public int speed;
    public float attackDistanceThreshold = 5f;
    public float timeBetweenAttacks = 2f;
    public float damage = 1;
    public float wanderRadius = 20f;
    public float wanderTimer = 3f;

    // Particle effects
    public ParticleSystem blood;

    // Sound effects
    public AudioClip moanSound;
    private AudioSource zombie_audioSource;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        if (GameObject.FindGameObjectWithTag("Player") != null) {
            currentState = State.Chasing;
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            // Run OnTargetDeath when player dies
            targetEntity.OnDeath += OnTargetDeath;

            // Get Collision radiuses which use for calculate proper attack range
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
        timer = 0;
    }

    void Update() {
        timer += Time.deltaTime;
        switch (currentState) {
            case State.Patrolling:
                // Chose random position to walk to
                if (timer >= wanderTimer) {
                    Patroling();
                    timer = 0;
                }
                    break;
            case State.Chasing:
                // Wait for next attack time
                if (hasTarget) {
                    if (timer > timeBetweenAttacks) {
                        // Check if player in attack range
                        float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                        if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)) {
    
                            StartCoroutine(Attack());        
                        }
                        timer = 0;
                    }
                }
                else currentState = State.Patrolling;
                break;
            case State.Attacking:
                // Do something between attack
                break;
            default:
                timer = 0;
                break;
        }
    }

    private void Patroling() {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, whatIsGround);
        agent.SetDestination(newPos);
        print(newPos);
        Debug.DrawRay(transform.position, newPos, Color.red);
    }

    void OnTargetDeath() {
        hasTarget = false;
        currentState = State.Patrolling;
    }

    IEnumerator UpdatePath() {
        float refreshRate = .25f;

        while (hasTarget) {
            if (currentState == State.Chasing) {
                // Get the direction of target
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                // Subtract by self collision radius, target's collision radius, and attack distance to prevent enemy from going into player body
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead) {
                    agent.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator Attack() {
        // Set state to attcking and disenble pathfinder
        currentState = State.Attacking;
        agent.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        // Attack position is around center of player body
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        // Prevent player from damaged multiple times.
        bool hasAppliedDamage = false;

        while (percent <= 1) {

            if (percent >= .5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            // Parabola function
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        // Set back to Chasing state and enable pathfinder
        currentState = State.Chasing;
        agent.enabled = true;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistanceThreshold);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, LayerMask layermask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
