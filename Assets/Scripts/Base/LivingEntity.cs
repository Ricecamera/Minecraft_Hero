using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class LivingEntity : MonoBehaviour, IDamageable {

    protected float health;
    protected bool dead;

    public float startingHealth;
    public event Action OnDeath;

    protected virtual void Start() {
        health = startingHealth;
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
        if (OnDeath != null) {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }

    public void setHealth(int newHealth) {
        if (newHealth > 0)
            health = newHealth;
        else
            health = 1;
    }
}

