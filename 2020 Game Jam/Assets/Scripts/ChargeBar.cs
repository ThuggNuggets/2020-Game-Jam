using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    public RawImage chargeBlock;
    public float maxScale = 2.714f; // Max scale of the box inside the bar

    private float minScale = 0.0f;
    private float keyHoldTimer = 0f;
    private Vector3 chargeBarScale;
    private Vector3 temp;
    private Kick kick;


    // Start is called before the first frame update
    void Start()
    {
        kick = FindObjectOfType<Kick>();
        chargeBarScale = chargeBlock.rectTransform.localScale;
        temp = new Vector3(1.0f, keyHoldTimer, 1.0f);
        chargeBarScale.y = minScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Math to make the timer match the scale of the bar
        keyHoldTimer = (kick.keyHoldTime / 2) * maxScale;

        temp.y = keyHoldTimer;

        // Boundary check
        if (temp.y > maxScale)
            temp.y = maxScale;

        // Invert the block
        temp.y = maxScale - temp.y;

        chargeBlock.rectTransform.localScale = temp;
    }
}
