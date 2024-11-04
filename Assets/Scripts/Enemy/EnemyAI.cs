using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IEnclosable, IChargeable
{
    [Header("General")]
    [SerializeField] private float _retargetingDelay;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _fleeRange;
    [SerializeField] private bool _allowEncloseKill = true;
    [SerializeField] private bool _allowChargeKill = true;
    [SerializeField] private LayerMask _viewBlocking;
    [SerializeField] private float _deathShrinkDuration;

    [Header("Attacking")]
    [SerializeField] private bool _debugAllowAttacking = true;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private Projectile _projectile;
    [SerializeField] private AudioEvent _attackSFX;

    [Header("Patrolling")]
    [SerializeField] private float _patrolRetargetMinDelay;
    [SerializeField] private float _patrolRetargetMaxDelay;
    [SerializeField] private float _patrolDistance;

    [Header("Animators")]
    [SerializeField] private RuntimeAnimatorController _maleAnim;
    [SerializeField] private RuntimeAnimatorController _femaleAnim;

    private NavMeshAgent _navAgent;
    private SegmentDetector _segmentDetector;
    private Animator _anim;
    private AudioSource _audio;

    private FollowSegment _nearest;

    private float _curPatrolDelay;
    private bool _hasPatrolTarget;

    private bool _canAttack = true;
    private bool _isDead;

    public bool CanEnclose { get; private set; } = true;

    private void Awake()
    {
        _segmentDetector = GetComponentInChildren<SegmentDetector>();

        _navAgent = GetComponent<NavMeshAgent>();

        _audio = GetComponent<AudioSource>();

        _anim = GetComponent<Animator>();

        float mOrFAnim = Random.value;
        if (mOrFAnim > 0.5f)
        {
            _anim.runtimeAnimatorController = _maleAnim;
        }
        else
        {
            _anim.runtimeAnimatorController = _femaleAnim;
        }

        _navAgent.updateRotation = false;
        _navAgent.updateUpAxis = false;
    }

    private void Start()
    {
        StartCoroutine(DelayedUpdate());
        StartCoroutine(ResetAttack());
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

            if(_isDead) {  continue; }

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
            else if (dist < _attackRange && HasLineOfSight(_nearest.transform.position)) // Line of sight check
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

    private bool HasLineOfSight(Vector3 target)
    {
        var diff = target - transform.position;
        return !Physics2D.Raycast(transform.position, diff.normalized, diff.magnitude, _viewBlocking);
    }

    private void Attack()
    {
        if (!_canAttack || !_debugAllowAttacking) { return; }

        _anim.SetBool("Walk", false);
        _anim.SetTrigger("Attack");

        var projectile = Instantiate(_projectile, transform.position, Quaternion.identity);
        projectile.Direction = (_nearest.transform.position - transform.position).normalized;

        if(_attackSFX) { _attackSFX.Play(_audio); }

        StartCoroutine(ResetAttack());
    }

    private void Chase()
    {
        _anim.SetBool("Walk", true);

        _navAgent.SetDestination(_nearest.transform.position);
    }

    private void Flee()
    {
        _anim.SetBool("Walk", true);

        // Get direction going from nearest segment to enemy. This is the direction to run away from the player
        Vector3 dir = (new Vector3(transform.position.x, transform.position.y, 0) - _nearest.transform.position).normalized;

        _navAgent.SetDestination(transform.position + (15 * dir));
    }

    private void RandomWalk()
    {
        _anim.SetBool("Walk", true);

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
        CanEnclose = false;
        StartDeath();
    }

    public void OnCharge()
    {
        if (_allowChargeKill)
        {
            StartDeath();
        }
    }

    public void StartDeath()
    {
        _isDead = true;
        var tween = transform.DOScale(0, _deathShrinkDuration);
        tween.SetEase(Ease.InOutCubic);
        tween.onComplete += () => Destroy(gameObject);
    }
}
