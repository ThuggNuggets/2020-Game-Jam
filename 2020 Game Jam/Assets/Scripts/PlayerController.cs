using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

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
    #region Knockback
    //public float invulnerabilityTime = 0.5f;
    //public float knockBackForce = 150f;
    //public float knockBackTime = 0.45f;
    //private float knockBackCounter;
    //public float slowDownAmount = 0.2f;
    #endregion
    #region Main Menu
    //public string MainMenuName = "Main Menu";
    //public bool DeathToMenu = false;
    //public float DeathToMenuTimer = 4;
    //private float DeathToMenuIterator = 0;
    #endregion
    #region Materials & Textures
    //public Material invulnerable;
    //public Material vulnerable;
    //public GameObject ProtagMesh;
    //public GameObject ProtagMeshBodyTexture;
    #endregion

    public int scoreForKill = 100;
    public bool disableMove = false;
    public GameObject kickBox;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject chargeBar;
    public TextMeshProUGUI scoreText;

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
    }

    void Update()
    {
        if (disableMove)
            return;

        if (transform.position.y > startingHeight)
            transform.position = new Vector3(transform.position.x, startingHeight, transform.position.z);

        #region Movement Update
        // Get the direction of input from the user
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
        Vector3 mousePosition3D = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - transform.position.y));
        transform.LookAt(mousePosition3D);

        charRigidbody.velocity = moveVelocity;
        #endregion

        #region Old Stuff
        //    if (inputDir != Vector2.zero)
        //    {
        //        ***THIS CODE LOCKS THE PLAYER ROTATION WHEN ATTACKING ANIMATION IS PLAYING***
        //        if (!(meleeSwipe.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) && !meleeSwipe.IsInTransition(0))


        //            if (!movementDisabled && !disableMove)
        //            {

        //                // Move the character relevant to the set current speed
        //                //controller.Move((transform.forward * currentSpeed) * Time.deltaTime);
        //            }
        //            else
        //            {
        //                Vector3 dir = new Vector3(inputDir.x, 0, inputDir.y);

        //                if (!movementDisabled && !disableMove)
        //                    charRigidbody.MovePosition((dir * currentSpeed) * Time.deltaTime);
        //            }
        //    }
        //}

        //if (!movementDisabled && !disableMove)
        //    charRigidbody.MovePosition(velocity * Time.deltaTime);
        #endregion

        #region Knockback Update
        //// Subtract the velocity by the slowDownAmount to slow down the knockback
        //velocity -= velocity * slowDownAmount;

        //// If the velocity gets below a certain threshold, set it to zero
        //if (velocity.magnitude < 0.35f)
        //    velocity = Vector3.zero;

        //// Only use the timer if the counter has been activated
        //if (knockBackCounter > 0)
        //{
        //    movementDisabled = true;
        //    knockBackCounter -= Time.deltaTime;
        //}
        //// Once the Counter reaches 0, removes the force applied to the enemy
        //else if (knockBackCounter <= 0)
        //{
        //    movementDisabled = false;
        //}
        #endregion

        #region Health Update
        //if (currentHealth > maxHealth)
        //    currentHealth = maxHealth;
        //else if (currentHealth <= 0)
        //    currentHealth = 0;
        #endregion

        if (Input.GetKeyDown(KeyCode.Escape) && deathMenu.activeSelf == false)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    #region Player Damage
    // When the enemy hits the player, the player takes damage
    //void OnCollisionEnter(Collision other)
    //{
    //    Debug.Log("Collision");
    //    Vector3 playerHitDirection = other.transform.forward /*other.transform.position - transform.position*/;
    //    playerHitDirection = playerHitDirection.normalized;

    //    if (other.gameObject.tag == "EnemySword")
    //    {
    //        //TrooperBehaviour enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<TrooperBehaviour>();
    //        //TrooperBehaviour enemy = other.gameObject.GetComponentInParent<TrooperBehaviour>();
    //        TrooperBehaviour enemy = other.gameObject.GetComponent<Weapon>().Owner;
    //        if (enemy.xIsDownedX == false)
    //        {
    //            playerWasDamaged = true;
    //            KnockBack(playerHitDirection);
    //            currentHealth -= enemy.enemyAttackStrength;
    //        }
    //    }
    //}


    // Function to call when the player takes damage
    //void PlayerTookDamage()
    //{
    //    // If the enemy took damage turn off the box collider
    //    if (playerWasDamaged)
    //    {
    //        if (timer == 0)
    //        {

    //            Debug.Log("PLAYERHURT");
    //            PlayerTookDamageAudio.Play();
    //        }

    //        PlayerInvulnerabilityOn();
    //        timer += Time.deltaTime;
    //    }

    //    // And turn it back on after half a second (or change to be after the spin attack is finished)
    //    if (timer >= invulnerabilityTime)
    //    {
    //        PlayerInvulnerabilityOff();
    //        playerWasDamaged = false;
    //    }
    //}

    //void PlayerInvulnerabilityOn()
    //{
    //    playerCollider.enabled = false;
    //    Debug.Log("Collider.enabled = " + playerCollider.enabled);
    //    ProtagMeshBodyTexture.GetComponent<SkinnedMeshRenderer>().material = invulnerable;
    //}

    //void PlayerInvulnerabilityOff()
    //{
    //    playerCollider.enabled = true;
    //    timer = 0;
    //    Debug.Log("Collider.enabled = " + playerCollider.enabled);
    //    ProtagMeshBodyTexture.GetComponent<SkinnedMeshRenderer>().material = vulnerable;
    //}

    //public void KnockBack(Vector3 direction)
    //{
    //    knockBackCounter = knockBackTime;

    //    playerMoveDirection = direction * knockBackForce;

    //    // Apply velocity relative to the direction the player has been knocked back
    //    velocity += playerMoveDirection;
    //}
    #endregion

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
        Time.timeScale = 0;
        deathMenu.SetActive(true);
    }
}