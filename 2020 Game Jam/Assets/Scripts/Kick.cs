using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour
{
    [Range(0, 5000)]
    public float kickForce = 100.0f;
    [Range(0.0f, 2.0f)]
    public float upwardsForce = 1.3f;
    public AudioSource kickSoundLight;
    public AudioSource kickSoundHeavy;
    private GameObject kickBox;
    private GameObject player;
    [HideInInspector]
    public float keyHoldTime;
    [Range(0.5f, 5.0f)]
    public float keyHoldMaxTime = 2.0f;
    private readonly float keyPressTime = 0.35f;
    private bool kickCharging = false;

    enum KickType
    {
        none,
        lightKick,
        chargeKick
    }
    KickType kickState;

    private void Start()
    {
        kickBox = this.gameObject;
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        //if (Input.GetButtonDown("Interact")) // Check for key press
        //    keyHoldTime = 0;
        //if (Input.GetButton("Interact")) // or key hold
        //    keyHoldTime += Time.deltaTime;

        //if (keyHoldTime < medvialPressTime)
        //    medvialPressed = true;

        CheckInteractKey();

        InteractKeyClear();

        Debug.Log(kickState);
    }

    private void OnTriggerStay(Collider other)
    {
        //Rigidbody enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Rigidbody>();

        if (other.CompareTag("Enemy"))
        {
            if (kickState == KickType.lightKick && Input.GetMouseButtonUp(0))
            {
                other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(kickForce, kickBox.transform.position + -player.transform.forward, 5.0f, upwardsForce, ForceMode.Force);
                kickSoundLight.Play();
            }
            else if (kickState == KickType.chargeKick && Input.GetMouseButtonUp(0))
            {
                other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(kickForce * keyHoldTime, kickBox.transform.position + -player.transform.forward, 5.0f, upwardsForce, ForceMode.Force);
                kickSoundHeavy.Play();
            }
            //enemy.AddExplosionForce((player.GetComponent<Rigidbody>().velocity.magnitude <= 0.1f ? kickForce : (kickForce * player.GetComponent<Rigidbody>().velocity.magnitude)), 
            //    kickBox.transform.position + -player.transform.forward, 5.0f);
            Debug.Log("Enemy in range");
        }
    }

    void CheckInteractKey()
    {
        if (Input.GetMouseButtonDown(0))
            keyHoldTime = 0;
        if (Input.GetMouseButton(0))
        {
            kickCharging = true;
            keyHoldTime += Time.deltaTime;
            if (keyHoldTime >= keyHoldMaxTime)
                keyHoldTime = keyHoldMaxTime;
        }

        //Debug.Log(keyHoldTime);

        if (kickCharging && keyHoldTime > keyPressTime)
            kickState = KickType.chargeKick;
        else if (keyHoldTime < keyPressTime && Input.GetMouseButtonDown(0) && kickState == KickType.none)
            kickState = KickType.lightKick;
    }

    void InteractKeyClear()
    {
        if (Input.GetMouseButtonUp(0))
        {
            keyHoldTime = 0;
            kickState = KickType.none;
        }
    }
}
