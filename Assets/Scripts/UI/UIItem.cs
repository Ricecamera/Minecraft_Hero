using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    private Image spriteImage;
    private int magazine = -1;
    private int ammo = -1;

    public Text ammoText;
    // Start is called before the first frame update
    void Start()
    {
        spriteImage = GetComponent<Image>();
    }

    public void UpdateGun(Shooting gun) {
        if (gun != null) {
            spriteImage.color = Color.white;
            spriteImage.sprite = gun.icon;

            int newMag = gun.magazineSize;
            int newAmmo = -1;
            if (gun is AmmoGun) {
                AmmoGun ammoGun = gun as AmmoGun;
                newAmmo = ammoGun.totalAmmo - gun.magazineSize;
            }
            Debug.Log("Add " + gun.gunTitle);
            UpdateText(newMag, newAmmo);
        }
        else {
            spriteImage.color = Color.clear;
            UpdateText(-1, -1);
            Debug.Log("No gun");
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
