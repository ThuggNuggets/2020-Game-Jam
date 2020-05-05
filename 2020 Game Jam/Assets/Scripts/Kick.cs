using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using XboxCtrlrInput;


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
    public AudioSource gruntLight;
    public AudioSource gruntHeavy;
    #endregion
    public GameObject blood; 
    public GameObject bloodParticle;
    public MeshRenderer leg;
    public PlayerController controller;

    // Xbox Controller Input
    [Header("Input")]
    public XboxButton kickButton;
    [Range(0, 1)]
    public int mouseButton = 0;

    #region Private Variables
    private GameObject kickBox;
    private GameObject player;
    private readonly float keyPressTime = 0.75f;
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
        player = GameObject.Find("Body");
        tempKnockBackTimer = knockBackTimer;
        leg = GameObject.Find("LegR").GetComponent<MeshRenderer>();
        CameraShaker.Instance.ShakeOnce(7f, 6f, 0.5f, 0.5f);
    }

    private void Update()
    {
        CheckInteractKey();
        KnockbackOnKick();
        InteractKeyClear();

        //Debug.Log(kickState);
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
                gruntLight.Play();
                StartCoroutine(SlowTime());
                Instantiate(blood,other.transform);
                Instantiate(bloodParticle, other.transform);
                CameraShaker.Instance.ShakeOnce(2f, 2f, 0.1f, 0.1f);
            }
            // Check for Heavy Kick Collision
            else if (kickState == KickType.heavyKick && Input.GetMouseButtonUp(0))
            {
                // If keyHoldTime is less than 0.1 second (0.1x multiplier) then just use 0.1x min force, otherwise heavyKickForce * keyHoldTime
                other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(heavyKickForce * ((keyHoldTime < 0.1f) ? 0.1f : keyHoldTime), kickBox.transform.position + -player.transform.forward, 5.0f, upwardsForce, ForceMode.Force);
                kickSoundHeavy.Play();
                gruntHeavy.Play();
                StartCoroutine(SlowTimeHeavy());
                Instantiate(blood, other.transform);
                Instantiate(bloodParticle, other.transform);
                CameraShaker.Instance.ShakeOnce(7f, 6f, 0.2f, 0.2f);
            }

            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.SetStunned();
        }
    }

    void CheckInteractKey()
    {
        if (GetInputButtonDown())
            keyHoldTime = 0;
        if (GetInputButtonStay())
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
            if (GetInputButtonUp())
            {
                heavyKickUsed = true;
                kickCharging = false;
                kickCharged = false;
                timerGoDown = false;
            }
        }
        // -=-LIGHT KICK-=-
        else if (keyHoldTime < keyPressTime && !kickCharged && GetInputButtonDown() && kickState == KickType.none)
        {
            kickState = KickType.lightKick;
        }
    }

    // Reset some values when button is released
    void InteractKeyClear()
    {
        if (GetInputButtonUp())
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

    private bool GetInputButtonDown()
    {
        if (controller.xboxController == XboxController.First)
        {
            bool r = XCI.GetButtonDown(kickButton, XboxController.First);
            if (r)
                return true;
            return r;
        }
        else
            return Input.GetMouseButtonDown(mouseButton);
    }

    private bool GetInputButtonStay()
    {
        if (controller.xboxController == XboxController.First)
            return XCI.GetButton(kickButton, XboxController.First);
        else
            return Input.GetMouseButton(mouseButton);
    }

    private bool GetInputButtonUp()
    {
        if (controller.xboxController == XboxController.First)
            return XCI.GetButtonUp(kickButton, XboxController.First);
        else
            return Input.GetMouseButtonUp(mouseButton);
    }

    IEnumerator SlowTime()
    {
        Time.timeScale = 0.7f;
        leg.enabled = true;
        yield return new WaitForSeconds(0.1f);

        Time.timeScale = 1f;
        leg.enabled = false;
    }
    IEnumerator SlowTimeHeavy()
    {
        Time.timeScale = 0.3f;
        leg.enabled = true;
        yield return new WaitForSeconds(0.1f);
        leg.enabled = false;
        Time.timeScale = 1f;

    }
}
