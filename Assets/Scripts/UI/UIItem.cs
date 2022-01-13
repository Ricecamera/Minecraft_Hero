using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    private int magazine = -1;
    private int ammo = -1;

    private Image spriteImage;
    private Shooting gun;

    public Text ammoText;
    public ReloadCircle reloadCircle;
    // Start is called before the first frame update
    void Start()
    {
        spriteImage = GetComponent<Image>();
    }

    private void TriggerReloadCircle(float duration) {
        reloadCircle.SetFillRate(duration);
        reloadCircle.StartLoad();
    }

    public void UpdateGun(Shooting gun) {

        if (gun != null) {
            // Remove old listener
            if (this.gun != null)
                this.gun.OnReload.RemoveListener(TriggerReloadCircle);

            // Set new gun
            this.gun = gun;
            spriteImage.color = Color.white;
            spriteImage.sprite = gun.icon;

            int newMag = gun.currentMagazine;
            int newAmmo = -1;
            if (gun is AmmoGun) {
                AmmoGun ammoGun = gun as AmmoGun;
                newAmmo = ammoGun.reserveAmmo;
            }
            //Debug.Log("Add " + gun.gunTitle);
            UpdateText(newMag, newAmmo);

            this.gun.OnReload.AddListener(TriggerReloadCircle);
        }
        else {
            if (this.gun != null) {
                this.gun.OnReload.RemoveListener(TriggerReloadCircle);
                this.gun = null;
            }
            spriteImage.color = Color.clear;
            UpdateText(-1, -1);
            //Debug.Log("No gun");
        }
        
    }

    public void UpdateText(int newMag, int newAmmo) {
        magazine = newMag;
        ammo = newAmmo;

        if (ammo == -1) {
            if (magazine == -1) {
                ammoText.text = "- / -";
            }
            else {
                ammoText.text = magazine + " / ∞";
            }
        }
        else {
            ammoText.text = magazine + " / " + ammo;
        }
    }
}
