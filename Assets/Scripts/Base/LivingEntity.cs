using UnityEngine;
using UnityEngine.Events;
using System;


public class LivingEntity : MonoBehaviour, IDamageable {

    protected float health;
    protected bool dead;

    [SerializeField]
    protected float startingHealth, maxHealth;

    [SerializeField]
    protected float deathDelay = .75f;
    public UnityEvent OnDeath;

    public bool Dead {
        get {return dead;}
    }

    void Awake() {
        health = startingHealth;
    }
    protected virtual void Start() {
        if (OnDeath == null)
            OnDeath = new UnityEvent();
    }

    public virtual void TakeHit(float damage, RaycastHit hit) {
        // Do some stuff here with hit var
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage) {
        health -= damage;

        if (health <= 0 && !dead) {
            
            Die(deathDelay);
        }
    }

    public virtual void Die(float delay) {
        dead = true;
        OnDeath?.Invoke();
        OnDeath.RemoveAllListeners();
        GameObject.Destroy(gameObject, delay);
    }

    public void setHealth(float newHealth) {
        if (newHealth > 0)
            this.health = (newHealth > maxHealth) ? maxHealth: newHealth;
        else
            this.health = 1;
    }
}

