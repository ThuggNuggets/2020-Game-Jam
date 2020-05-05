using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public List<Transform> obstacles = new List<Transform>();

    [Header("General Properties")]
    public float maxVelocity = 8.0f;
    public float acceleration = 10.0f;
    public float attackDistance = 2.0f;
    public float stunnedAfterKickTime = 2.0f;

    [Header("Kick Properties")]
    public float kickForce = 25.0f;
    public float kickCooldown = 1.0f;
    public float timeBeforeKick = 1.0f;

    [Header("Avoid Properties")]
    public LayerMask obstacleLayer;
    public float avoidAngle = -120.0f;
    public float avoidRecheckTime = 0.7f;
    public float avoidRayDistance = 2.0f;

    // Private instances / variables:
    private Rigidbody _rigidbody;
    private Rigidbody _playerRigidbody;
    private Vector3 _direction = Vector3.zero;
    private Vector3 _avoidDirection = Vector3.zero;
    private Vector3 _kickDirection = Vector3.zero;
    private Vector3 _seekPosition = Vector3.zero;
    private Transform _transform;
    private Transform _closestHole;
    private bool _justKicked = false;
    private bool _hasBeenKicked = false;
    private float _lock_y_height = 0.0f;
    private float _kickTimer = 0.0f;
    private float _beforeKickTimer = 0.0f;
    private float _stunnedTimer = 0.0f;

    public enum AIState
    {
        Stunned,
        Chase,
        Attack,
        Death,
        Count,
    }
    private AIState _currentState;

    private void Awake()
    {
        // Getting components
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody>();

        // Setting variables
        _currentState = AIState.Chase;
        _kickTimer = kickCooldown;
        _beforeKickTimer = timeBeforeKick;
        _lock_y_height = _transform.position.y;
    }

    private void Start()
    {
        // Getting player rigidbody
        _playerRigidbody = playerTransform.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Called every physics updated
    /// </summary>
    private void FixedUpdate()
    {
        if (_transform.position.y > _lock_y_height)
            _transform.position = new Vector3(_transform.position.x, _lock_y_height, _transform.position.z);

        // Updating the current state
        switch (_currentState)
        {
            case AIState.Stunned:
                Stunned();
                break;

            case AIState.Chase:
                Chase();
                break;

            case AIState.Attack:
                Attack();
                break;

            case AIState.Death:
                Death();
                break;
        }
    }

    /// <summary>
    /// Updates while stunned
    /// </summary>
    private void Stunned()
    {
        _hasBeenKicked = true;
        _stunnedTimer -= Time.fixedDeltaTime;
        if (_stunnedTimer <= 0.0f)
        {
            _stunnedTimer = stunnedAfterKickTime;
            _currentState = AIState.Chase;
            _hasBeenKicked = false;
        }
    }

    /// <summary>
    /// Updates while chasing
    /// </summary>
    private void Chase()
    {
        // Check if player is in attack range:
        if (GetDistanceToPlayer() > attackDistance)
        {
            // Find the seek and kick direction:
            _closestHole = FindNearestHoleToPlayer();
            _kickDirection = (_closestHole.position - playerTransform.position).normalized;
            _seekPosition = (playerTransform.position - (_kickDirection * 2.0f));

            // Rotate enemy to direction:
            Vector3 dir = GetDirection(_seekPosition);
            dir.y = 0.0f;
            _transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            // Add force:
            if (_rigidbody.velocity.magnitude < maxVelocity)
               _rigidbody.AddForce(dir * acceleration * Time.fixedDeltaTime, ForceMode.Impulse);
        }
        else
        {
            // Enter the attack state if they are:
            _currentState = AIState.Attack;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(_seekPosition, 1.0f);
    }

    /// <summary>
    /// Updates while attacking
    /// </summary>
    private void Attack()
    {
        // Rotate enemy to direction:
        if(_kickDirection.magnitude > 0)
            _transform.rotation = Quaternion.LookRotation(_kickDirection, Vector3.up);

        if (GetDistanceToPlayer() < attackDistance)
        {
            Debug.Log("Kicking");
            _rigidbody.velocity = Vector3.zero;
            _beforeKickTimer -= Time.deltaTime;
            if (!_justKicked && _beforeKickTimer <= 0.0f)
            {
                _playerRigidbody.AddForce(_kickDirection * kickForce, ForceMode.Impulse);
                _justKicked = true;
                _beforeKickTimer = timeBeforeKick;
            }
        }
        else
        {
            if (!_justKicked)
            {
                _currentState = AIState.Chase;
                _beforeKickTimer = timeBeforeKick;
            }
        }

        if (_justKicked)
        {
            _kickTimer -= Time.fixedDeltaTime;
            if(_kickTimer <= 0.0f)
            {
                _kickTimer = kickCooldown;
                _currentState = AIState.Chase;
                _justKicked = false;
                _beforeKickTimer = timeBeforeKick;
            }
        }
    }

    /// <summary>
    /// Updates while dead
    /// </summary>
    private void Death()
    {

    }

    /// <summary>
    /// Gets the required direction to the player while compensating for obstacles.
    /// </summary>
    /// <param name="seekPos">Direction to seek.</param>
    public Vector3 GetDirection(Vector3 seekPos)
    {
        _direction = (seekPos - _transform.position).normalized;
        if (_avoidDirection.magnitude <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(_transform.position, _transform.forward, out hit, avoidRayDistance))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    _rigidbody.velocity = Vector3.zero;
                    _avoidDirection = Quaternion.AngleAxis(avoidAngle, Vector3.up) * hit.normal;
                    StartCoroutine(CheckSequence());
                    return _avoidDirection;
                }
                return _direction;
            }
            else
                return _direction;
        }
        else
            return _avoidDirection;
    }

    /// <summary>
    /// Returns the distance to the player.
    /// </summary>
    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(playerTransform.position, _transform.position);
    }

    /// <summary>
    /// Timer for checking for another obstacle.
    /// </summary>
    public IEnumerator CheckSequence()
    {
        yield return new WaitForSecondsRealtime(avoidRecheckTime);
        _avoidDirection = Vector3.zero;
    }

    /// <summary>
    /// Loops through the obstacle list and checks for the hole nearest to the player.
    /// </summary>
    public Transform FindNearestHoleToPlayer()
    {
        int index = 0;
        float distance = Vector3.Distance(obstacles[0].position, playerTransform.position);
        for (int i = 1; i < obstacles.Count; ++i)
        {
            float calDis = Vector3.Distance(obstacles[i].position, playerTransform.position);
            if(calDis < distance)
            {
                distance = calDis;
                index = i;
            }
        }
        return obstacles[index];
    }
    
    /// <summary>
    /// Forces the state system to go into the stunned state.
    /// </summary>
    public void SetStunned()
    {
        _stunnedTimer = stunnedAfterKickTime;
        _currentState = AIState.Stunned;
    }

    /// <summary>
    /// Returns true if the enemy has been kicked.
    /// </summary>
    public bool HasBeenKicked
    {
        get { return _hasBeenKicked; }
    }

    /// <summary>
    /// Sets up references to other enemy components.
    /// </summary>
    /// <param name="playerTransform"> The player. </param>
    /// <param name="obstacles"> The holes. </param>
    public void SetupReferences(Transform playerTransform, List<Transform> obstacles)
    {
        this.playerTransform = playerTransform;
        this.obstacles = obstacles;
    }
}