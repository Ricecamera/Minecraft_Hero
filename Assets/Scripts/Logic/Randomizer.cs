using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Randomizer : MonoBehaviour
{
    public static Randomizer instance = null;

    public List<AmmoGun> containGuns = new List<AmmoGun>();
    public List<Item> containItems = new List<Item>();

    public float machineGunChance = .5f,
                  shotgunChance = .4f,
                  rocketLauncherChance = .1f;

    private void Awake() {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public AmmoGun getRandomGun() {
        float value = Random.Range(0f, 1f);
        AmmoGun selectedGun;
        if (value <= machineGunChance) {
            selectedGun = containGuns[0];
        }
        else if (machineGunChance < value && value < machineGunChance + shotgunChance) {
            selectedGun = containGuns[1];
        }
        else {
            selectedGun = containGuns[2];
        }
        return selectedGun;
    }

    public Item getRandomItem() {
        float value = Random.Range(0f, 1f);
        int randIndex = 0;
        if (value > 0.8f) {
            randIndex = 1;
        }
        if (value > 0.5f) {
            randIndex = 2;
        }
        return containItems[randIndex];
    }
}
