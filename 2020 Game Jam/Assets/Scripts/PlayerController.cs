using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Health
    public int currentHealth = 6;
    public int maxHealth = 14;

    #endregion
    #region Movement
    public float walkSpeed = 5;
    //public float runSpeed = 10; // For Debug purposes [REMOVE IN ALPHA]
    public float turnSmoothTime = 0.1f;
    public float speedSmoothTime = 0.1f;
    public float gravityModifier = 10.0f;
    #endregion
    #region Knockback
    public float invulnerabilityTime = 0.5f;
    public float knockBackForce = 150f;
    public float knockBackTime = 0.45f;
    private float knockBackCounter;
    public float slowDownAmount = 0.2f;
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

    public bool disableMove = false;

    float turnSmoothVelocity;
    float speedSmoothVelocity;
    float currentSpeed;
    public GameObject kickBox;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject chargeBar;
    #region Private Variables
    //private CharacterController controller;
    private Rigidbody charRigidbody;
    private CapsuleCollider playerCollider;

    private float fallAmount;
    private Vector3 velocity;
    private Vector3 gravity;
    private Vector2 input;
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
        gravity = Physics.gravity * gravityModifier;

        playerCollider = GetComponent<CapsuleCollider>();
        charRigidbody = GetComponent<Rigidbody>();
        startingHeight = transform.position.y;
        fallAmount = startingHeight + 5.0f;
    }

    void Update()
    {
        //if(pauseMenu.activeSelf == false)
        #region Movement Update
        // Get the direction of input from the user
        input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        
        // Normalize the input
        Vector2 inputDir = input.normalized;

        bool movementDisabled = false;
        // Debug addition to get around faster
        //bool running = Input.GetKey(KeyCode.LeftShift);
        // Set to walkSpeed in alpha test
        float targetSpeed = (/*(running) ? runSpeed : */walkSpeed) * inputDir.magnitude;
        // Speed up the player overtime when they move
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        if (inputDir != Vector2.zero)
        {
            /* ***THIS CODE LOCKS THE PLAYER ROTATION WHEN ATTACKING ANIMATION IS PLAYING***
            if (!(meleeSwipe.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) && !meleeSwipe.IsInTransition(0))
            {*/
            // Set the target rotation to be equal to the direction that the player is facing
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
            // Change the rotation to the player to be equal to that direction with smoothing
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            charRigidbody.MovePosition(transform.position + transform.forward * currentSpeed * Time.deltaTime);
            
            //if (!movementDisabled && !disableMove)
            //{

            //    // Move the character relevant to the set current speed
            //    //controller.Move((transform.forward * currentSpeed) * Time.deltaTime);
            //}
            //else
            //{
            //    Vector3 dir = new Vector3(inputDir.x, 0, inputDir.y);

            //    if (!movementDisabled && !disableMove)
            //        //controller.Move((dir * currentSpeed) * Time.deltaTime);
            //        charRigidbody.MovePosition((dir * currentSpeed) * Time.deltaTime);
            //}
            //}
        }

        //if (!movementDisabled && !disableMove)
        //    //controller.Move(velocity * Time.deltaTime);
        //    charRigidbody.MovePosition(velocity * Time.deltaTime);
        #endregion

        #region Knockback Update
        // Add gravity to the player
        velocity += gravity * Time.deltaTime;

        // Subtract the velocity by the slowDownAmount to slow down the knockback
        velocity -= velocity * slowDownAmount;

        // If the velocity gets below a certain threshold, set it to zero
        if (velocity.magnitude < 0.35f)
            velocity = Vector3.zero;

        // Only use the timer if the counter has been activated
        if (knockBackCounter > 0)
        {
            movementDisabled = true;
            knockBackCounter -= Time.deltaTime;
        }
        // Once the Counter reaches 0, removes the force applied to the enemy
        else if (knockBackCounter <= 0)
        {
            movementDisabled = false;
        }
        #endregion

        #region Health Update
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
            currentHealth = 0;
        #endregion

        if (Input.GetKeyDown(KeyCode.Escape) && deathMenu.activeSelf == false)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DPlane"))
        {
            StartCoroutine(Death());
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

    private IEnumerator Death()
    {
        //Play Death Audio

        //Turn off Player Charge Bar
        chargeBar.SetActive(false);
        
        //stop spawning enemies
        
        yield return new WaitForSeconds(2);

        //Turn Dead screen on
        deathMenu.SetActive(true);
    }
}