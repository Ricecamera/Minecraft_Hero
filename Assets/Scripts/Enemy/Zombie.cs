using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : LivingEntity {
    public enum State {Wandering, Chasing, Attacking, Death };

    private State currentState;
    private float myCollisionRadius;
    private float targetCollisionRadius;
    private float timer;

    private NavMeshAgent agent;
    private Transform target;
    private LivingEntity targetEntity;

    public int speed;
    public float attackDistanceThreshold = 5f;
    public float timeBetweenAttacks = 1.5f;
    public float damage = 1;
    public float wanderRadius = 20f;
    public float wanderTimer = 2f;

    // Particle effects
    public ParticleSystem blood;

    // Sound effects
    public AudioClip moanSound;
    private AudioSource zombie_audioSource;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();

        // if zombie doesn't have NavMesh, do nothing
        if (agent == null) return;
        
        if (GameObject.FindGameObjectWithTag("Player") != null) {
            currentState = State.Chasing;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            // Run OnTargetDeath when player dies
            targetEntity.OnDeath.AddListener(OnTargetDeath);

            // Get Collision radiuses which use for calculate proper attack range
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
        else {
            currentState = State.Wandering;
        }
        timer = 0;
        agent.speed = speed;
    }

    void Update() {
        timer += Time.deltaTime;
        switch (currentState) {
            case State.Wandering:
                // Chose random position to walk to
                if (timer >= wanderTimer) {
                    Wandering();
                    timer = 0;
                }
                    break;
            case State.Chasing:
                // Wait for next attack time
                if (target != null) {
                    if (timer > timeBetweenAttacks) {
                        // Check if player in attack range
                        float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                        if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2) && !targetEntity.Dead) {
    
                            StartCoroutine(Attack());        
                        }
                        timer = 0;
                    }
                }
                else currentState = State.Wandering;
                break;
            case State.Attacking:
                // Do something between attack
                break;
            default:
                timer = 0;
                break;
        }
    }

    private void Wandering() {
        if (!dead) {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            agent?.SetDestination(newPos);
        }
    }

    void OnTargetDeath() {
        targetEntity.OnDeath.RemoveListener(OnTargetDeath);
        print("Player has died");
        currentState = State.Wandering;
        Wandering();
    }

    IEnumerator UpdatePath() {
        float refreshRate = .25f;

        while (target != null) {
            if (currentState == State.Chasing) {
                // Get the direction of target
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                // Subtract by self collision radius, target's collision radius, and attack distance to prevent enemy from going into player body
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead) {
                    agent?.SetDestination(targetPosition);
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

        while (percent <= 1 && !dead) {

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

    public Vector3 RandomNavSphere(Vector3 origin, float dist) {
        Vector3 finalPosition = Vector3.zero;
        Vector3 randPosition = Random.insideUnitSphere * dist;
        randPosition += origin;

        if (NavMesh.SamplePosition(randPosition, out NavMeshHit navHit, dist, 1)) {
            finalPosition = navHit.position;
        }
        print(finalPosition);
        return finalPosition;
    }

    public override void Die() {
        currentState = State.Death;
        base.Die();
    }
}
