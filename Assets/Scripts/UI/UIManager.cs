using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Overlay UI")]
    public Text lifeText;
    public Text scoreText;

    public Text gunIndexText;
    public UIItem item;
    [Space(10)]

    [Header("Game over UI")]
    public GameObject gameOverUI;
    public GameObject pauseGameUI;


    public void UpdateScore(long newScore) {
       scoreText.text = "Score: " + newScore;
    }

    public void UpdateLife(int newLife) {
        lifeText.text = "x " + newLife;
    }

    public void UpdateGun(Shooting gun, int currentIdx) {
        item.UpdateGun(gun);
        gunIndexText.text = "< " + currentIdx + " / " + GunController.MAX_GUN + " >";
    }

    public void UpdateAmmo(int mag, int ammo) {
        item.UpdateText(mag, ammo);
    }

    public void ShowGameOverUI(bool isShow) {
        gameOverUI.SetActive(isShow);
    }

    public void ShowPauseUI(bool isShow) {
        pauseGameUI.SetActive(isShow);
    }
}
