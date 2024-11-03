using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IEnclosable
{
    [Header("General")]
    [SerializeField] private float _retargetingDelay;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _fleeRange;
    [SerializeField] private bool _allowEncloseKill = true;

    [Header("Attacking")]
    [SerializeField] private float _attackSpeed;
    [SerializeField] private Projectile _projectile;

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

    public bool CanEnclose { get; private set; } = true;

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
        CanEnclose = _allowEncloseKill;
    }

    private void Update()
    {
        if (_curPatrolDelay > 0)
        {
            _curPatrolDelay -= Time.deltaTime;
        }
    }
    private IEnumerator DelayedUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(_retargetingDelay);

            _navAgent.isStopped = false;
            _nearest = _segmentDetector.GetNearest();
            if (_nearest == null)
            {
                RandomWalk();
                continue;
            }
            _hasPatrolTarget = false;

            var dist = (_nearest.transform.position - transform.position).magnitude;

            if (dist < _fleeRange)
            {
                Flee();
            }
            else if (dist < _attackRange)
            {
                _navAgent.isStopped = true;
                Attack();
            }
            else
            {
                Chase();
            }
        }
    }

    private void Attack()
    {
        if (!_canAttack) { return; }

        var projectile = Instantiate(_projectile, transform.position, Quaternion.identity);

        projectile.Direction = (_nearest.transform.position - transform.position).normalized;

        StartCoroutine(ResetAttack());
    }

    private void Chase()
    {
        _navAgent.SetDestination(_nearest.transform.position);
    }

    private void Flee()
    {
        // Get direction going from nearest segment to enemy. This is the direction to run away from the player
        Vector3 dir = (new Vector3(transform.position.x, transform.position.y, 0) - _nearest.transform.position).normalized;

        _navAgent.SetDestination(transform.position + (15 * dir));
    }

    private void RandomWalk()
    {
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

    public void Enclose()
    {
        Destroy(gameObject);
        CanEnclose = false;
    }
}
