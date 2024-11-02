using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float _retargetingDelay;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _fleeRange;

    [Header("Attacking")]
    [SerializeField] private float _attackSpeed;

    [Header("Patrolling")]
    [SerializeField] private float _patrolRetargetMinDelay;
    [SerializeField] private float _patrolRetargetMaxDelay;
    [SerializeField] private float _patrolDistance;

    private NavMeshAgent _navAgent;
    private SegmentDetector _segmentDetector;

    private FollowSegment _nearest;

    private float _curPatrolDelay;
    private bool _hasPatrolTarget;

    private bool _canAttack = true;

    private void Awake()
    {
        _segmentDetector = GetComponentInChildren<SegmentDetector>();

        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.updateRotation = false;
        _navAgent.updateUpAxis = false;
    }

    private void Start()
    {
        StartCoroutine(DelayedUpdate());
    }
    private void Update()
    {
        
    }
    private IEnumerator DelayedUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(_retargetingDelay);

            _nearest = _segmentDetector.GetNearest();
            if (_nearest == null)
            {
                Debug.Log("must've been the wind");
                RandomWalk();
                continue;
            }
            _hasPatrolTarget = false;

            var dist = (_nearest.transform.position - transform.position).magnitude;

            if (dist < _fleeRange)
            {
                Debug.Log("AHH! RUn away!!");
                _navAgent.isStopped = false;
                Flee();
            }
            else if (dist < _attackRange)
            {
                Debug.Log("Dodge THIS!");
                _navAgent.isStopped = true;
                Attack();
            }
            else
            {
                _navAgent.isStopped = false;
                Debug.Log("omw");
                Chase();
            }
        }
    }

    private void Attack()
    {
        if (!_canAttack) { return; }
        // attack stuff here pew pew
        ResetAttack();
    }

    private void Chase()
    {
        _navAgent.SetDestination(_nearest.transform.position);
    }

    private void Flee()
    {
        // Get direction going from nearest segment to enemy. This is the direction to run away from the player
        Vector3 dir = (new Vector3(transform.position.x, transform.position.y, 0) - _nearest.transform.position).normalized;

        Debug.Log($"Run dir: {dir}");
        // Run arbitrarily far in that direction
        _navAgent.SetDestination(transform.position + (15 * dir));
    }

    private void RandomWalk()
    {
        if (_curPatrolDelay > 0)
        {
            _curPatrolDelay -= Time.deltaTime;
        }

        if (_navAgent.remainingDistance < .1 && _hasPatrolTarget)
        {
            _hasPatrolTarget = false;
            _curPatrolDelay = Random.Range(_patrolRetargetMaxDelay, _patrolRetargetMaxDelay);
        }

        if (!_hasPatrolTarget && _curPatrolDelay <= 0)
        {
            _navAgent.SetDestination(GetRandomPatrolPoint());
            _hasPatrolTarget = true;
        }
    }
    private Vector3 GetRandomPatrolPoint()
    {
        var randX = Random.Range(-1f, 1f);
        var randY = Random.Range(-1f, 1f);

        Vector3 dir = (new Vector3(randX, randY, 0f)).normalized;

        var dist = Random.Range(0, _patrolDistance);

        return transform.position + (dir * dist);
    }

    private IEnumerator ResetAttack()
    {
        _canAttack = false;
        yield return new WaitForSeconds(1 / _attackSpeed);
        _canAttack = true;
    }
    //private StateMachine _stateMachine;

    //private void Awake()
    //{
    //    _navAgent = GetComponent<NavMeshAgent>();
    //    _navAgent.updateRotation = false;
    //    _navAgent.updateUpAxis = false;

    //    _stateMachine = new StateMachine();

    //    var patrolState = new PatrolState(_navAgent, _patrolRetargetMin, _patrolRetargetMax, _patrolDistance);

    //    _stateMachine.SetState(patrolState);
    //}

    //private void Update()
    //{
    //    _stateMachine.Tick();
    //}

}
