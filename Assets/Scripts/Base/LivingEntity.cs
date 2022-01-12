using UnityEngine;
using UnityEngine.Events;
using System;


public class LivingEntity : MonoBehaviour, IDamageable {

    protected float health;
    protected bool dead;

    [SerializeField]
    protected float startingHealth, maxHealth;
    public UnityEvent OnDeath;

    public bool Dead {
        get {return dead;}
    }

    protected virtual void Start() {
        health = startingHealth;
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
            float deathDelay = .75f;
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
            health = (newHealth > maxHealth) ? maxHealth: newHealth;
        else
            health = 1;
    }
}

