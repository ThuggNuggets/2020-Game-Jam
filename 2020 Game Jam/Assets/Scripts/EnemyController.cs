using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public List<Transform> holes = new List<Transform>();

    [Header("General Properties")]
    public float movementSpeed = 10.0f;
    public float attackDistance = 2.0f;
    public float kickForce = 25.0f;
    public float kickCooldown = 1.0f;

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
    private Transform _closestHole;
    private bool _justKicked = false;
    private float _kickTimer = 0.0f;

    public enum AIState
    {
        Idle,
        Chase,
        Attack,
        Death,
        Count,
    }
    private AIState _currentState;

    private void Awake()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
        _playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        _currentState = AIState.Chase;
        _kickTimer = kickCooldown;
    }

    private void FixedUpdate()
    {
        switch(_currentState)
        {
            case AIState.Idle:
                break;

            case AIState.Chase:
                Chase();
                break;

            case AIState.Attack:
                Attack();
                break;

            case AIState.Death:
                break;
        }
    }

    private void Chase()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) > attackDistance)
        {
            _closestHole = GetClosestHoleToPlayer();
            _kickDirection = (_closestHole.position - playerTransform.position).normalized;
            Vector3 playerGrounded = playerTransform.position;
            playerGrounded.y = this.transform.position.y;
            Vector3 seekPos = (playerGrounded - (_kickDirection * 2.0f));

            transform.LookAt(playerGrounded);
            _rigidbody.MovePosition(transform.position + GetDirection(seekPos) * movementSpeed * Time.fixedDeltaTime);
        }
        else
        {
            _currentState = AIState.Attack;
        }
    }

    private void Attack()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) < attackDistance)
        {
            if(!_justKicked)
            {
                _playerRigidbody.AddForce((_kickDirection + Vector3.up).normalized * kickForce, ForceMode.Impulse);
                _justKicked = true;
            }
        }
        else
        {
            if (!_justKicked)
            {
                _currentState = AIState.Chase;
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
            }
        }
    }

    public Vector3 GetDirection(Vector3 seekPos)
    {
        _direction = (seekPos - transform.position).normalized;
        if (_avoidDirection.magnitude <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, avoidRayDistance))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
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

    public IEnumerator CheckSequence()
    {
        yield return new WaitForSecondsRealtime(avoidRecheckTime);
        _avoidDirection = Vector3.zero;
    }

    public Transform GetClosestHoleToPlayer()
    {
        int index = 0;
        float distance = Vector3.Distance(holes[0].position, playerTransform.position);
        for (int i = 1; i < holes.Count; ++i)
        {
            float calDis = Vector3.Distance(holes[i].position, playerTransform.position);
            if(calDis < distance)
            {
                distance = calDis;
                index = i;
            }
        }
        return holes[index];
    }
}