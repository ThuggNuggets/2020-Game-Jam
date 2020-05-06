using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using XboxCtrlrInput;

public class PlayerController : MonoBehaviour
{
    #region Health
    public int currentHealth = 6;
    public int maxHealth = 14;

    #endregion
    #region Movement
    public float walkSpeed = 5;
    readonly float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    #endregion

    public float stunnedAfterKickTime = 1.0f;
    public int scoreForKill = 100;
    public bool disableMove = false;
    public GameObject kickBox;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject chargeBar;
    public TextMeshProUGUI scoreText;

    // Xbox Controller Input
    [Header("Xbox Controller Input")]
    public float turnSensitivity = 100.0f;
    public float smoothing = 2.0f;
    public XboxController xboxController;
    private Vector2 smoothV = Vector2.zero;

    private float _stunnedTimer = 0.0f;
    private bool _isStunned = false;
    private Vector3 _lookDirection = Vector3.zero;
    #region Private Variables
    private Rigidbody charRigidbody;
    private CapsuleCollider playerCollider;
    private Vector2 input;
    private Vector3 moveVelocity;
    private int totalScore = 0;
    #endregion

    #region Hidden Variable
    [HideInInspector]
    public bool buttonPressed = false;
    [HideInInspector]
    public bool gamePaused;
    [HideInInspector]
    public float startingHeight;
    [HideInInspector]
    public float swipeIterator = 0;
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool playerWasDamaged;

    #endregion

    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        charRigidbody = GetComponent<Rigidbody>();
        startingHeight = transform.position.y;

        // Checking for any connected controllers and defaulting to first if true:
        if (FindConnectedControllers())
            xboxController = XboxController.First;
        else
            xboxController = (XboxController)(-1);
    }

    void Update()
    {
        if(_isStunned)
        {
            disableMove = true;
            _stunnedTimer += Time.deltaTime;
            if(_stunnedTimer >= stunnedAfterKickTime)
            {
                _isStunned = false;
                disableMove = false;
            }
        }

        if (disableMove)
            return;

        if (transform.position.y > startingHeight)
            transform.position = new Vector3(transform.position.x, startingHeight, transform.position.z);

        #region Movement Update
        // Get the direction of input from the user
        if (xboxController == XboxController.First)
            input = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, XboxController.First), XCI.GetAxis(XboxAxis.LeftStickY, XboxController.First));
        else
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        moveVelocity = new Vector3(input.x, 0.0f, input.y) * currentSpeed;
        moveVelocity.y = charRigidbody.velocity.y;

        // Normalize the input
        Vector2 inputDir = input.normalized;

        //bool movementDisabled = false;
        float targetSpeed = walkSpeed * inputDir.magnitude;
        // Speed up the player overtime when they move
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        // Set the target rotation to be equal to the direction of the mouse

        if (_lookDirection.magnitude > 0.2F)
            transform.forward = _lookDirection;

        if (xboxController == XboxController.First)
        {
            var lookDelta = new Vector2(XCI.GetAxis(XboxAxis.RightStickX, XboxController.First), XCI.GetAxis(XboxAxis.RightStickY, XboxController.First));
            lookDelta = Vector2.Scale(lookDelta, new Vector2(turnSensitivity * smoothing, turnSensitivity * smoothing));

            // Getting the interpolated result between the two float values.
            smoothV.x = Mathf.Lerp(smoothV.x, lookDelta.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, lookDelta.y, 1f / smoothing);

            // Calculating the transforms rotation based on the rotation axis:
            if(smoothV.magnitude > 0.1F)
                _lookDirection = new Vector3(smoothV.x, 0f, smoothV.y);
            else
                _lookDirection = new Vector3(input.x, 0f, input.y);
        }
        else
        {
            Vector3 mousePosition3D = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - transform.position.y));
            transform.LookAt(mousePosition3D);
        }

        charRigidbody.velocity = moveVelocity;
        #endregion

        if (GetStartButtonPress() && deathMenu.activeSelf == false)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private bool GetStartButtonPress()
    {
        if (xboxController == XboxController.First)
            return XCI.GetButton(XboxButton.Start, XboxController.First);
        else
            return Input.GetKeyDown(KeyCode.Escape);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            charRigidbody.AddForce(Vector3.down * 10.0f, ForceMode.Impulse);
            disableMove = true;
            charRigidbody.velocity = Vector3.zero;
            HasDied();
        }
    }

    private bool FindConnectedControllers()
    {
        // Getting the amound of controllers plugged in:
        int connectedControllers = XCI.GetNumPluggedCtrlrs();

        // Printing the amount of connected controllers to the log:
        if (connectedControllers == 0)
            Debug.Log("No Xbox controllers plugged in!");
        else
        {
            Debug.Log(connectedControllers + " Xbox controllers plugged in.");
        }

        // Printing the controller names:
        XCI.DEBUG_LogControllerNames();

        // Returning true if any controllers found:
        return connectedControllers > 0;
    }

    public void SetStunned()
    {
        _isStunned = true;
    }

    public void KilledEnemy()
    {
        totalScore += scoreForKill;
        scoreText.text = totalScore.ToString();
    }

    public void HasDied()
    {
        StartCoroutine(Death());
    }


    private IEnumerator Death()
    {
        //Play Death Audio 
        
        //Turn off Player Charge Bar 
        chargeBar.SetActive(false);

        //stop spawning enemies 

        yield return new WaitForSeconds(2);

        //Turn Dead screen on 
        //Time.timeScale = 0;
        deathMenu.SetActive(true);
    }
}