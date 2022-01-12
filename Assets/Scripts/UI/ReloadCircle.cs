using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadCircle : MonoBehaviour
{
    [SerializeField]
    private Image circleImg;
    [SerializeField]
    private float fillRate;

    [Range(0, 1)]
    private float progress = 0f;
    private bool loading;

    private void Start() {
        Reset();
    }
    private void Update() {
        if (loading) {
            progress += fillRate * Time.deltaTime;
            if (progress < 1f) {

                circleImg.fillAmount = progress;
            }
            else {
                Reset();
            }
        }
    }

    public void SetFillRate(float fillDuration) {
        fillRate = 1.0f/fillDuration;
    }
    
    public void Reset() {
        fillRate = 0;
        progress = 0;
        loading = false;
        circleImg.fillAmount = progress;
        gameObject.SetActive(false);
    }

    public void StartLoad() {
        loading = true;
        gameObject.SetActive(true);
    }
}
