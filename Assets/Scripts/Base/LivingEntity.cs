using UnityEngine;
using UnityEngine.Events;
using System;


public class LivingEntity : MonoBehaviour, IDamageable {

    protected float health;
    protected bool dead;

    public float startingHealth;
    public UnityEvent OnDeath;

    public bool Dead {
        get {return dead;}
    }

    protected virtual void Start() {
        health = startingHealth;
        if (OnDeath == null)
            OnDeath = new UnityEvent();
    }

    public void TakeHit(float damage, RaycastHit hit) {
        // Do some stuff here with hit var
        TakeDamage(damage);
    }

    public void TakeDamage(float damage) {
        health -= damage;

        if (health <= 0 && !dead) {
            Die();
        }
    }

    public virtual void Die() {
        dead = true;
        OnDeath?.Invoke();
        GameObject.Destroy(gameObject, 1.5f);
    }

    public void setHealth(int newHealth) {
        if (newHealth > 0)
            health = newHealth;
        else
            health = 1;
    }
}

