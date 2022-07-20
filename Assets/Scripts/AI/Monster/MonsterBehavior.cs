using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterAnimator))]
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterBehavior : Health, IImpulse
{

    [Header("Idle =============================")]
    [SerializeField]
    Vector2 idleStateLengthRange = new Vector2( 1, 3f );
    [Header("Patrol =============================")]
    [SerializeField]
    Vector2 patrolStateLengthRange = new Vector2( 4f, 6f );
    [SerializeField]
    float patrolRadius = 2f, patrolStoppingDistance = 0.5f;
    float maxPatrolSpeed;
    [SerializeField]
    bool drawPatrolRadius = true;
    [Header("Chase =============================")]
    [SerializeField]
    Vector2 maxChaseSpeedRange = new Vector2( 5f, 8f );
    float maxChaseSpeed;
    [Header("Attack =============================")]
    [SerializeField]
    float attackRadius = 2f;
    [SerializeField]
    Vector2 attackDamageRange = new Vector2(25f, 40f);
    [SerializeField]
    bool drawAttackRadius = true;


    [Header("Misc =============================")]
    [SerializeField]
    Texture2D[] textureVariations;
    [SerializeField]
    SoundEffectPlayer deathSFX, hitSFX, normalSFX;
    [SerializeField]
    LayerMask groundLayer;
    [SerializeField]
    float groundCheckDistance = 0.1f, groundCheckRadialOffset = 0f, groundCheckVerticalOffset = 0f;


    static PlayerHealth player;
    static IImpulse playerImpulse;
    MonsterAnimator animator;
    CapsuleCollider normalCollider;
    BoxCollider deathCollider;

    new SkinnedMeshRenderer renderer;
    MaterialPropertyBlock mpb;

    AIViewCone viewCone;
    NavMeshAgent navMeshAgent;
    Rigidbody r3d;

    public MonsterBehaviorState CurrentState { get; private set; } = MonsterBehaviorState.Idle;
    public bool IsGrounded { get; private set; } = true;

    bool isPhysicsEnabled = false;
    public bool IsPhysicsEnabled {
        get { return isPhysicsEnabled; }
        set {
            if( value ) {
                navMeshAgent.enabled = false;
                r3d.isKinematic = false;
            } else {
                navMeshAgent.enabled = true;
                r3d.isKinematic = true;
            }
            isPhysicsEnabled = value;
        }
    }

    private void Awake() {
        animator = GetComponent<MonsterAnimator>();
        normalCollider = GetComponent<CapsuleCollider>();
        deathCollider = GetComponent<BoxCollider>();
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        viewCone = GetComponent<AIViewCone>();
        r3d = GetComponent<Rigidbody>();
        mpb = new MaterialPropertyBlock();

        if( player == null ) {
            player = FindObjectOfType<PlayerHealth>();
            playerImpulse = player.GetComponent<IImpulse>();
        }

        viewCone.OnView += OnViewPlayer;
        maxChaseSpeed = Random.Range(maxChaseSpeedRange.x, maxChaseSpeedRange.y);
        maxPatrolSpeed = maxChaseSpeed / 2f;
    }

    private void OnDestroy() {
        viewCone.OnView -= OnViewPlayer;
    }

    private void Start() {
        Respawn();
    }

    void OnViewPlayer() {
        if(!navMeshAgent.enabled)
            return;
        this.StopAllCoroutines();
        ChangeState(MonsterBehaviorState.Chasing);
    }

    void ChangeState( MonsterBehaviorState newState ) {
        if(!IsAlive){
            CurrentState = MonsterBehaviorState.Dead;
            DeadState();
            return;
        }
        CurrentState = newState;
        switch( CurrentState ) {
            case MonsterBehaviorState.Idle:
                IdleState();
                break;
            case MonsterBehaviorState.Patrol:
                PatrolState();
                break;
            case MonsterBehaviorState.Chasing:
                ChaseState();
                break;
            case MonsterBehaviorState.Attack:
                AttackState();
                break;
            case MonsterBehaviorState.Hurt:
                HurtState();
                break;
            default: case MonsterBehaviorState.None:
                viewCone.IsEnabled = true;
                this.StopAllCoroutines();
                animator.StopMoving();
                break;
        }
    }

    void IdleState() {
        viewCone.IsEnabled = true;
        navMeshAgent.isStopped = true;
        animator.StopMoving();

        this.StopAllCoroutines();
        SetSpeed(0f);
        idleStateRoutine = IdleStateRoutine(Random.Range(idleStateLengthRange.x, idleStateLengthRange.y));
        this.StartCoroutine(idleStateRoutine);
    }
    IEnumerator idleStateRoutine;
    IEnumerator IdleStateRoutine(float length) {
        yield return new WaitForSeconds(length);
        if( Random.Range( 0, 10 ) == 0 ) {
            normalSFX.Play();
        }
        ChangeState(MonsterBehaviorState.Patrol);
    }

    void PatrolState() {
        viewCone.IsEnabled = true;
        animator.StartMoving();
        navMeshAgent.isStopped = false;

        navMeshAgent.stoppingDistance = patrolStoppingDistance;
        Vector3 patrolTarget = (Random.onUnitSphere * patrolRadius) + transform.position;
        NavMesh.SamplePosition(patrolTarget, out NavMeshHit navMeshResult, patrolRadius, NavMesh.AllAreas);
        navMeshAgent.SetDestination(navMeshResult.position);

        this.StopAllCoroutines();
        SetSpeed( maxPatrolSpeed );
        patrolStateRoutine = PatrolStateRoutine(Random.Range(patrolStateLengthRange.x, patrolStateLengthRange.y));
        this.StartCoroutine(patrolStateRoutine);
    }
    IEnumerator patrolStateRoutine;
    IEnumerator PatrolStateRoutine(float length) {
        // dont start counting down until path is finalized
        while(navMeshAgent.pathPending) { yield return null; }
        float timer = 0f;
        bool reachedDestination() {
            return
                navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
            ;
        }
        while( !reachedDestination() && timer < length ) {
            timer += Time.deltaTime;
            yield return null;
        }
        ChangeState(MonsterBehaviorState.Idle);
    }

    void ChaseState() {
        viewCone.IsEnabled = false;
        animator.StartMoving();
        navMeshAgent.isStopped = false;

        navMeshAgent.stoppingDistance = attackRadius;

        this.StopAllCoroutines();
        SetSpeed(maxChaseSpeed);
        chaseStateRoutine = ChaseStateRoutine();
        this.StartCoroutine(chaseStateRoutine);
    }
    IEnumerator chaseStateRoutine;
    IEnumerator ChaseStateRoutine() {
        NavMeshHit playerPosition;
        NavMesh.SamplePosition(
            player.transform.position,
            out playerPosition,
            viewCone.ViewConeLength,
            NavMesh.AllAreas
        );
        navMeshAgent.SetDestination(playerPosition.position);
        bool recalculate = true;
        bool reachedDestination() {
            Vector3 direction = player.transform.position - transform.position;
            bool withinRange = direction.magnitude <= attackRadius;
            bool lookingAt = Vector3.Dot(direction.normalized, transform.forward) > 0.6f;
            return withinRange && lookingAt;
        }
        while( !reachedDestination() ) {
            recalculate = !recalculate;
            if( Vector3.Distance(transform.position, player.transform.position) > viewCone.ViewConeLength ) {
                ChangeState(MonsterBehaviorState.Idle);
                yield break;
            }
            else if(recalculate) {
                NavMesh.SamplePosition(
                    player.transform.position,
                    out playerPosition,
                    viewCone.ViewConeLength,
                    NavMesh.AllAreas
                );
                navMeshAgent.SetDestination(playerPosition.position);
                recalculate = false;
            }
            yield return null;
        }
        ChangeState(MonsterBehaviorState.Attack);
    }

    void AttackState() {
        viewCone.IsEnabled = false;
        StopForced();
        animator.Attack();

        this.StopAllCoroutines();
        attackStateRoutine = AttackStateRoutine();
        this.StartCoroutine(attackStateRoutine);
    }
    IEnumerator attackStateRoutine;
    IEnumerator AttackStateRoutine() {
        Vector3 direction = player.transform.position - transform.position;
        bool withinRange = direction.magnitude <= attackRadius;
        bool lookingAt = Vector3.Dot(direction.normalized, transform.forward) > 0.6f;
        if(withinRange && lookingAt) {
            player.Damage(Random.Range(attackDamageRange.x, attackDamageRange.y));
            Vector3 impulseDirection = (player.transform.position - transform.position).normalized;
            playerImpulse.Impulse((impulseDirection + Vector3.up) * 10f);
        }
        yield return new WaitForSeconds(0.45f);
        normalSFX.Play();
        ChangeState(MonsterBehaviorState.Chasing);
    }

    void HurtState() {
        viewCone.IsEnabled = false;
        animator.TakeDamage();
        if(navMeshAgent.enabled)
            StopForced();

        this.StopAllCoroutines();
        hurtStateRoutine = HurtStateRoutine();
        this.StartCoroutine(hurtStateRoutine);
    }
    IEnumerator hurtStateRoutine;
    IEnumerator HurtStateRoutine() {
        yield return new WaitForSeconds(0.24f);
        hitSFX.Play();
        normalSFX.Play();
        if(navMeshAgent.enabled)
            ChangeState(MonsterBehaviorState.Chasing);
        else
            ChangeState(MonsterBehaviorState.None);
    }

    void DeadState() {
        if(navMeshAgent.enabled) {
            StopForced();
            deathSFX.Play();
        }
        animator.Death();
        normalCollider.enabled = false;
        deathCollider.enabled = true;
        viewCone.IsEnabled = false;
        navMeshAgent.enabled = false;

        this.StopAllCoroutines();
        deathRoutine = DeathRoutine();
        this.StartCoroutine(deathRoutine);
    }
    IEnumerator deathRoutine;
    IEnumerator DeathRoutine() {
        yield return new WaitForSeconds(2f);
    }

    void SetSpeed(float newSpeed) {
        if(setSpeedRoutine != null)
            this.StopCoroutine(setSpeedRoutine);
        setSpeedRoutine = SetSpeedRoutine(newSpeed);
        this.StartCoroutine(setSpeedRoutine);
    }
    IEnumerator setSpeedRoutine;
    IEnumerator SetSpeedRoutine(float newSpeed) {
        float timer = 0f;
        float length = 0.2f;
        float oldSpeed = navMeshAgent.speed;
        while(timer < length) {
            float t = timer / length;
            navMeshAgent.speed = Mathf.Lerp(oldSpeed, newSpeed, t);
            animator.SetMovementSpeed(navMeshAgent.speed / maxChaseSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        navMeshAgent.speed = newSpeed;
        animator.SetMovementSpeed(navMeshAgent.speed);
    }

    void StopForced() {
        navMeshAgent.speed = 0f;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;
        animator.StopMoving();
        animator.SetMovementSpeed(0f);
    }

    private void FixedUpdate() {
        ApplyGravity();
    }

    public void Respawn() {
        ResetHealth();
        ChangeState(MonsterBehaviorState.Idle);
        IsPhysicsEnabled = false;
        normalCollider.enabled = true;
        deathCollider.enabled = false;
        mpb.SetTexture("_MainTex", textureVariations[Random.Range(0, textureVariations.Length)]);
        renderer.SetPropertyBlock(mpb);
    }

    public override void Damage(float delta)
    {
        if( IsAlive ) {
            base.Damage(delta);
            if( CurrentState != MonsterBehaviorState.Attack )
                ChangeState(MonsterBehaviorState.Hurt);
        }
    }

    public enum MonsterBehaviorState {
        Idle, // stand around, if player seen, chase else timer until patrol
        Patrol, // randomly select target and walk there. if player seen, chase. when at destination, idle
        Chasing, // target player until within certain radius, then attack. if player gets too far, patrol
        Attack, // stop moving, attack
        Hurt, // take damage, then chase player
        Dead,
        None // placeholder until navmeshagent is ready
    }

    void ApplyGravity() {
        if( IsPhysicsEnabled ) {
            r3d.AddForce( Physics.gravity );
        } else {
            r3d.velocity = Vector3.zero;
        }
    }

    Vector3 forwardGroundCheckStart, backGroundCheckStart, leftGroundCheckStart, rightGroundCheckStart;
    bool GroundCheck() {
        RecalculateGroundCheckStart();

        return Physics.Raycast(forwardGroundCheckStart, Vector3.down, groundCheckDistance, groundLayer) ||
            Physics.Raycast(backGroundCheckStart, Vector3.down, groundCheckDistance, groundLayer) ||
            Physics.Raycast(leftGroundCheckStart, Vector3.down, groundCheckDistance, groundLayer) ||
            Physics.Raycast(rightGroundCheckStart, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void OnValidate()
    {
        if( navMeshAgent == null )
            navMeshAgent = GetComponent<NavMeshAgent>();
        RecalculateGroundCheckStart();
    }

    void RecalculateGroundCheckStart() {
        Vector3 verticalOffset = Vector3.up * groundCheckVerticalOffset;
        float radialScale = navMeshAgent.radius + groundCheckRadialOffset;
        forwardGroundCheckStart = transform.position + (( transform.forward * radialScale) + verticalOffset);
        backGroundCheckStart    = transform.position + ((-transform.forward * radialScale) + verticalOffset);
        leftGroundCheckStart    = transform.position + ((-transform.right   * radialScale) + verticalOffset);
        rightGroundCheckStart   = transform.position + (( transform.right   * radialScale) + verticalOffset);
    }

    public void Impulse(Vector3 force) {
        IsPhysicsEnabled = true;
        r3d.AddForce(force, ForceMode.Impulse);
        ChangeState(MonsterBehaviorState.None);
    }

    private void OnCollisionEnter(Collision other) {
        this.StartCoroutine(DisablePhysicsAfterCollision());
    }

    IEnumerator DisablePhysicsAfterCollision() {
        while (!GroundCheck()) { yield return null; }
        yield return new WaitForSeconds(0.001f);
        IsPhysicsEnabled = false;
        ChangeState(MonsterBehaviorState.Chasing);
    }

    private void OnDrawGizmosSelected() {
        DebugDrawGroundCheck();
        DebugPatrolRadius();
        DebugDrawAttackRadius();
    }

    void DebugDrawGroundCheck() {
        Gizmos.color = GroundCheck() ? Color.green : Color.red;
        Gizmos.DrawLine(forwardGroundCheckStart, forwardGroundCheckStart + (Vector3.down * groundCheckDistance));
        Gizmos.DrawLine(backGroundCheckStart, backGroundCheckStart + (Vector3.down * groundCheckDistance));
        Gizmos.DrawLine(leftGroundCheckStart, leftGroundCheckStart + (Vector3.down * groundCheckDistance));
        Gizmos.DrawLine(rightGroundCheckStart, rightGroundCheckStart + (Vector3.down * groundCheckDistance));
    }

    void DebugPatrolRadius() {
        if(!drawPatrolRadius)
            return;
        Gizmos.color = Color.green * 0.4f;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }

    void DebugDrawAttackRadius() {
        if(!drawAttackRadius)
            return;
        Gizmos.color = Color.red * 0.4f;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
