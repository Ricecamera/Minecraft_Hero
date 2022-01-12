using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : AmmoGun 
{
    [SerializeField]
    private float shotDistance, coneAngleWidth = 60;

    [SerializeField]
    private int rayCount = 7, shotDamage = 2;

    public LayerMask collisionMask;
    private void CheckHit() {
        float dTheta = coneAngleWidth / (rayCount - 1);
        for (int i = 1; i <= rayCount; i++) {
            // Get forward vector relative to gun direction
            Vector3 relativeVec = transform.forward;
            // Calculate the radian the relativeVec have to be rotate
            float radian = Mathf.Deg2Rad * (dTheta * (i - (rayCount + 1) / 2));
            // Rotate along Y-axis
            float newX = relativeVec.x * Mathf.Cos(radian) + relativeVec.z * Mathf.Sin(radian);
            float newZ = -relativeVec.x * Mathf.Sin(radian) + relativeVec.z * Mathf.Cos(radian);
            Vector3 dir = new Vector3(newX, relativeVec.y, newZ);

            // Create raycast at above direction
            Ray ray = new Ray(firePoint.position, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, shotDistance, collisionMask, QueryTriggerInteraction.Collide)) {
                OnHitObject(hit);
            }
            
        }
    }

    //protected override void Update() {
    //    float dTheta = coneAngleWidth / (rayCount - 1);
    //    for (int i = 1; i <= rayCount; i++) {
    //        // Get forward vector relative to gun direction
    //        Vector3 relativeVec = transform.forward;
    //        // Calculate the radian the relativeVec have to be rotate
    //        float radian = Mathf.Deg2Rad * (dTheta * (i - (rayCount + 1) / 2));
    //        // Rotate along Y-axis
    //        float newX = relativeVec.x * Mathf.Cos(radian) + relativeVec.z * Mathf.Sin(radian);
    //        float newZ = -relativeVec.x * Mathf.Sin(radian) + relativeVec.z * Mathf.Cos(radian);
    //        Vector3 dir = new Vector3(newX, relativeVec.y, newZ);

    //        Debug.DrawRay(firePoint.position, shotDistance * dir, Color.green);
    //    }
    //    base.Update();
    //}

    private void OnHitObject(RaycastHit hit) {
        print(hit.collider.gameObject.name);
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null) {
            damageableObject.TakeHit(shotDamage, hit);
        }
    }

    public override void Shoot() {

        // check if current Time is able to shoot
        if (CanShoot() && !IsMagazineEmpty()) {
            AudioManager.instance.PlaySingle(fireSound);
            CheckHit();

            // reduce bullets in magazine by one
            currentMagazine--;
            OnShoot?.Invoke(currentMagazine, reserveAmmo);
            ResetTimer();
        }

        if (IsMagazineEmpty()) {
            if (!IsBulletEmpty() && !isReloading) {
                AudioManager.instance.PlaySingle(reloadSound);
                StartCoroutine(Reload());
            }
            return;
        }
    }
}
