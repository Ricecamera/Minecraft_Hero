using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector2 inputVector;
    public Vector3 mousePosition;

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        inputVector = new Vector2(h, v);
        mousePosition = Input.mousePosition;
    }
}
