using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour
{
    #region Public Variables
    [Range(0, 5000)]
    public float lightKickForce = 20.0f;
    [Range(0, 5000)]
    public float heavyKickForce = 100.0f;
    [Range(0.0f, 2.0f)]
    public float upwardsForce = 1.1f;
    public float playerHeavyKickBackForce = 30.0f;
    [Range(0.5f, 5.0f)]
    public float keyHoldMaxTime = 2.0f;
    public float knockBackTimer = 0.1f;
    public AudioSource kickSoundLight;
    public AudioSource kickSoundHeavy;
    #endregion

    #region Private Variables
    private GameObject kickBox;
    private GameObject player;
    private readonly float keyPressTime = 0.35f;
    private float tempKnockBackTimer = 0.0f;
    private bool kickCharging = false;
    private bool kickCharged = false;
    private bool timerGoDown = false;
    private bool heavyKickUsed = false;
    private bool getDirection = false;
    private bool doPlayerKnockback = false;
    private Vector3 knockbackDirection;
    #endregion

    [HideInInspector]
    public float keyHoldTime;

    enum KickType
    {
        none,
        lightKick,
        heavyKick
    }
    KickType kickState;

    private void Start()
    {
        kickBox = this.gameObject;
        player = GameObject.Find("Player");
        tempKnockBackTimer = knockBackTimer;
    }

    private void Update()
    {
        CheckInteractKey();
        KnockbackOnKick();
        InteractKeyClear();

        Debug.Log(kickState);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Check for Light Kick Collision
            if (kickState == KickType.lightKick && Input.GetMouseButtonUp(0))
            {
                other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(lightKickForce, kickBox.transform.position + -player.transform.forward, 5.0f, upwardsForce, ForceMode.Force);
                kickSoundLight.Play();
            }
            // Check for Heavy Kick Collision
            else if (kickState == KickType.heavyKick && Input.GetMouseButtonUp(0))
            {
                // If keyHoldTime is less than 0.1 second (0.1x multiplier) then just use 0.1x min force, otherwise heavyKickForce * keyHoldTime
                other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(heavyKickForce * ((keyHoldTime < 0.1f) ? 0.1f : keyHoldTime), kickBox.transform.position + -player.transform.forward, 5.0f, upwardsForce, ForceMode.Force);
                kickSoundHeavy.Play();
            }
        }
    }

    void CheckInteractKey()
    {
        if (Input.GetMouseButtonDown(0))
            keyHoldTime = 0;
        if (Input.GetMouseButton(0))
        {
            kickCharging = true;

            // Make the bar go up and down
            if (!kickCharged && !timerGoDown || kickCharged && !timerGoDown)
            {
                keyHoldTime += Time.deltaTime;
            }
            else if (kickCharged && timerGoDown)
            {
                keyHoldTime -= Time.deltaTime;
                if (keyHoldTime <= 0.0f) // Reset the timer to go back up
                    timerGoDown = false;
            }

            // On first full hold of the charge bar
            if (keyHoldTime >= keyHoldMaxTime)
            {
                keyHoldTime = keyHoldMaxTime;
                kickCharged = true;
                timerGoDown = true;
                Debug.Log("KICK CHARGED");
            }
        }

        //Debug.Log(keyHoldTime);

        // -=-HEAVY KICK-=-
        if (kickCharging && keyHoldTime > keyPressTime || kickCharged)  // Need to check for holding heavy kick charging first                                                    
        {                                                               // before the light kick
            kickState = KickType.heavyKick;
            // Reset values when the play releases the mouse button
            if (Input.GetMouseButtonUp(0))
            {
                heavyKickUsed = true;
                kickCharging = false;
                kickCharged = false;
                timerGoDown = false;
            }
        }
        // -=-LIGHT KICK-=-
        else if (keyHoldTime < keyPressTime && !kickCharged && Input.GetMouseButtonDown(0) && kickState == KickType.none)
        {
            kickState = KickType.lightKick;
        }
    }

    // Reset some values when button is released
    void InteractKeyClear()
    {
        if (Input.GetMouseButtonUp(0))
        {
            keyHoldTime = 0;
            kickState = KickType.none;
        }
    }

    // Knock the player back upon using heavy kick
    void KnockbackOnKick()
    {
        if (heavyKickUsed)
        {
            // Get the direction to push player back just once
            if (!getDirection)
            {
                knockbackDirection = -transform.forward;
                getDirection = true;
            }

            knockBackTimer -= Time.deltaTime;

            // While the knockback timer hasn't reached zero, continue applying force on the player
            if (knockBackTimer > 0.0f)
            {
                // Only get player knockback if you get 75% power on the kick
                if (keyHoldTime >= (keyHoldMaxTime * 0.75f))
                    doPlayerKnockback = true;

                if (doPlayerKnockback)
                {
                    player.GetComponent<Rigidbody>().AddForce(knockbackDirection.normalized * playerHeavyKickBackForce, ForceMode.Impulse);
                    Debug.Log("KeyHoldTime release: " + keyHoldTime);
                }
            }

            // When it reaches zero, reset everything
            if (knockBackTimer <= 0.0f)
            {
                knockBackTimer = tempKnockBackTimer;
                heavyKickUsed = false;
                getDirection = false;
                doPlayerKnockback = false;
            }
        }

        //if (LightKickUsed)
        //{
        //    // Get the direction to push player back just once
        //    if (!getDirection)
        //    {
        //        knockbackDirection = -transform.forward;
        //        getDirection = true;
        //    }

        //    knockBackTimer -= Time.deltaTime;
        //    if (knockBackTimer > 0.0f)
        //    {
        //        player.GetComponent<Rigidbody>().AddForce(knockbackDirection.normalized * playerLightKickBackForce, ForceMode.Impulse);
        //    }

        //    if (knockBackTimer <= 0.0f)
        //    {
        //        knockBackTimer = tempKnockBackTimer;
        //        LightKickUsed = false;
        //        getDirection = false;
        //    }
        //}
    }
}
