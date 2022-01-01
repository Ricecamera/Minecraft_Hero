using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    public static CameraManager instance = null;    //Static instance of GameManager which allows it to be accessed by any other script.
    
    public MeshRenderer playArea;                   // bound area of the game

    public Vector3 Bound {
        get {
            return playArea.bounds.size / 2;
        }
    }

    void Awake() {
        //Singleton pattern

        //Check if instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // get ratio of screen and playArea
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = playArea.bounds.size.x / playArea.bounds.size.z;

        if (screenRatio >= targetRatio) {
            //Calculate orthographic size directly
            Camera.main.orthographicSize = playArea.bounds.size.z / 2;
        }
        else {
            //Normalize orgthographic size with ratio defference in size of target's ratio and screen's ratio
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = playArea.bounds.size.z / 2 * differenceInSize;
        }
    }
}
