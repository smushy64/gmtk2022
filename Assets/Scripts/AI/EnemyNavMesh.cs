using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyNavMesh : MonoBehaviour
{
    public int ScoreToAdd = 1;

    [Header("NavMesh Logics")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private LayerMask groundLayer, playerLayer;

    [SerializeField] private float groundCheckDistance = 1;
    [SerializeField] private float JumpSpeedIncreaseMultiplayer = 1;
    private float NormalSpeed;

    [SerializeField] private float timeBetweenAttacks;
    private bool AttackedBool;

    [SerializeField] private bool playerInChaseRange, playerInAttackRange;

    [Header("Sets a random patrol point inside this area")]
    [SerializeField] private bool patrolEnabled = false;
    [SerializeField] private float MaxPosX;
    [SerializeField] private float MinPosX, MaxPosZ, MinPosZ;

    [SerializeField] private Vector3 patrolPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointReachRange; //kinda like offset from the walkpoint to reach it

    [Header("AI Behavior")]
    public float Speed;

    [SerializeField] private float PatrolSpeedMultiplier = 1;
    [SerializeField] private float ChaseSpeedMultiplier = 1, AttackSpeedMultiplier = 1;

    [SerializeField] private float StopDistance;

    [SerializeField] private float ChasingRange, RangeToAttack;

    [SerializeField] private bool CanJump = false;
    [SerializeField] private bool IsRangedEnemy = false;
    [SerializeField] private bool IsFlyingEnemy = false;
    [SerializeField] private bool CanExplode = false;

    [Header("Attack")]
    [SerializeField] private float Damage;
    [SerializeField] private GameObject ProjectilePrefab, ExplodeParticle;
    [SerializeField] private Transform shootpoint;
    [SerializeField] private float ProjectileSpeed;
    [HideInInspector] public bool Exploded = false;

    public bool IsInAttackRange => playerInAttackRange;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (CanJump)
        {
            if (IsGrounded() == false && !Jumped)
            {
                Jumped = true;
                NormalSpeed = Speed;
                Speed = agent.speed * JumpSpeedIncreaseMultiplayer;
                StartCoroutine(Jump());
            }
            else if (IsGrounded() == true && Jumped)
            {
                Speed = NormalSpeed;
                agent.speed = Speed;
                Jumped = false;
            }
        }

        playerInChaseRange = Physics.CheckSphere(transform.position, ChasingRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, RangeToAttack, playerLayer);

        if (patrolEnabled && !playerInChaseRange && !playerInAttackRange) Patrolling();
        if (playerInChaseRange && !playerInAttackRange) ChasePlayer();
        if (playerInChaseRange && playerInAttackRange) AttackPlayer();
    }
    private void Patrolling()
    {
        IsChasing = false;
        OnAttack = false;
        agent.speed = Speed * PatrolSpeedMultiplier;

        if (!walkPointSet)
            SearchForWalkPoint();

        if (walkPointSet)
            agent.SetDestination(patrolPoint);

        Vector3 distanceToWalkPoint = transform.position - patrolPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchForWalkPoint()
    {
        float randomX = Random.Range(MinPosX, MaxPosX);
        float randomZ = Random.Range(MinPosZ, MaxPosZ);
        patrolPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(patrolPoint, -transform.up, 3f, groundLayer))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        if (!IsChasing)
        {
            IsChasing = true;
            OnAttack = false;
            EnemyActions.RemoveEnemyAttacking?.Invoke(this);
        }
        agent.speed = Speed * ChaseSpeedMultiplier;

        if (IsRangedEnemy && !IsFlyingEnemy)
            transform.LookAt(player.transform.position);

        agent.SetDestination(player.position);
    }
    private bool OnAttack = false, IsChasing = false;
    private void AttackPlayer()
    {
        if (!OnAttack)
        {
            OnAttack = true;
            IsChasing = false;
            EnemyActions.AddEnemyAttacking?.Invoke(this);
        }

        agent.stoppingDistance = StopDistance;
        agent.speed = Speed * AttackSpeedMultiplier;

        if (IsFlyingEnemy)
            shootpoint.transform.LookAt(player.transform.position);
        if (IsRangedEnemy && !IsFlyingEnemy)
            transform.LookAt(player.transform.position);

        // agent.SetDestination(player.position);

        if (!AttackedBool)
        {
            //attack is here

            if(IsRangedEnemy)
            {
                GameObject bullet = Instantiate(ProjectilePrefab, shootpoint.transform.position, shootpoint.rotation) as GameObject;
                bullet.GetComponent<Bullet>().Damage = Damage;
                bullet.GetComponent<Rigidbody>().AddForce(shootpoint.forward * ProjectileSpeed);
            }
            if (CanExplode)
            {
                // FindObjectOfType<EnemyManager>().ResetCombo();
                Exploded = true;
                Instantiate(ExplodeParticle, this.transform.position, Quaternion.identity);
                EnemyActions.OnEnemyKilled?.Invoke(this);
                Destroy(this.gameObject);
            }
            if(!IsRangedEnemy && !CanExplode && !IsFlyingEnemy)
            {
                Collider[] touching = Physics.OverlapSphere(this.transform.position, 1, playerLayer, QueryTriggerInteraction.Ignore); // chekcs if enemy is spawned inside an object
                if (touching.Length != 0)
                {
                    player.transform.gameObject.GetComponent<SimpleHealth>().TakeDamage(Damage);
                }
            }

            AttackedBool = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); //delays the attack
        }
    }
    private void ResetAttack()
    {
        AttackedBool = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RangeToAttack);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ChasingRange);
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundCheckDistance + 0.1f);
    }
    private bool Jumped = false;

    private IEnumerator Jump()
    {
        agent.baseOffset += 0.215f;
        for (int i = 0; i < 9; i++) // sets the offset to Y
        {
            agent.baseOffset += 0.215f;
            yield return new WaitForSeconds(.015f);
        }

        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < 10; i++) // sets the offset to Y
        {
            agent.baseOffset -= 0.215f;
            yield return new WaitForSeconds(.01f);
        }

        for (int i = 0; i < 10; i++) // adds a small like bouncy effect when it lands
        {
            if (i <= 4)
                transform.localScale = new Vector3(1, transform.localScale.y - .025f, 1);
            else
                transform.localScale = new Vector3(1, transform.localScale.y + .025f, 1);
            yield return new WaitForSeconds(.025f);
        }
    }
}
