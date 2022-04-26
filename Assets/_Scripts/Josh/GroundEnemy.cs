using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Authored by Joshua Hilliard
public class GroundEnemy : EnemyBase
{
    [HideInInspector] public Transform Player;
    NavMeshAgent agent;

    Animator animator;

    public float speed = 5;

    [SerializeField] private float detectionRange = 20;
    [SerializeField] private float meleeRange = 2;

    [SerializeField] private float attackInterval = 1.5f;
    private float attackTimer;

    private float distanceFromPlayer;

    private float AggroTimer;
    [SerializeField] private float AggroLength = 3;

    public LayerMask RaycastLayerIgnore;

    public Transform meleeAttackPosition;
    public float meleeAttackRange;

    public float meleeAttackDamage = 10;



    private enum State
    {
        Idle,
        Chasing,
        Attacking,
    }

    private State currentState = State.Idle;


    // Start is called before the first frame update
    void Start()
    {
        //Player = GameObject.Find("Player").transform;
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        attackTimer = attackInterval/2;
        AggroTimer = AggroLength;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(agent.destination);

        switch (currentState) 
        {
            case State.Idle:
                IdleState();
                break;
            case State.Chasing:
                ChasingState();
                break;
            case State.Attacking:
                AttackingState();
                break;
            

        }

        if (currentHealth <= 0)
        {
            Die();
        }

        distanceFromPlayer = Vector3.Distance(Player.position, transform.position);
        //agent.SetDestination(Player.position);
    }

    private void IdleState()
    {
        animator.SetInteger("moving", 0);
        animator.SetInteger("battle", 0);
        AggroTimer = AggroLength;
        agent.ResetPath();

        if (distanceFromPlayer <= detectionRange)
        {
            Vector3 DirectionToPlayer = Player.position - transform.position;
            if (Physics.Raycast(transform.position, DirectionToPlayer, out RaycastHit hit, detectionRange, ~RaycastLayerIgnore))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    currentState = State.Chasing;
                }
            }
        }
    }

    private void ChasingState()
    {
        animator.SetInteger("moving", 1);
        animator.SetInteger("battle", 1);

        agent.SetDestination(Player.position);

        Vector3 DirectionToPlayer = Player.position - gameObject.transform.position;

        if (distanceFromPlayer <= detectionRange)
        {

            if (Physics.Raycast(gameObject.transform.position, DirectionToPlayer, out RaycastHit hit, detectionRange, ~RaycastLayerIgnore))
            {

                if (hit.transform.CompareTag("Player"))
                {
                    AggroTimer = AggroLength;
                }


                if (!hit.transform.CompareTag("Player"))
                {
                    AggroTimer -= Time.deltaTime;
                    if (AggroTimer <= 0)
                    {
                        currentState = State.Idle;
                    }
                }
            }

        }
        else
        {
            AggroTimer -= Time.deltaTime;
            if (AggroTimer <= 0)
            {
                currentState = State.Idle;
            }
        }

        if(distanceFromPlayer <= meleeRange)
        {
            currentState = State.Attacking;
        }

    }

    private void AttackingState()
    {
        agent.SetDestination(Player.position);

        if (distanceFromPlayer <= meleeRange)
        {
            animator.SetInteger("moving", 0);
            attackTimer -= Time.deltaTime;
            if(attackTimer <= 0)
            {
                animator.SetInteger("moving", 2);
                attackTimer = attackInterval;
                Collider[] hitColliders = Physics.OverlapSphere(meleeAttackPosition.position, meleeAttackRange);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.CompareTag("Player"))
                    {
                        hitCollider.gameObject.GetComponent<PlayerStats>().TakeDamage(meleeAttackDamage);
                    }
                }
                //Debug.Log("Attack");
            }
        }

        if(distanceFromPlayer >= meleeRange)
        {
            currentState = State.Chasing;
        }
    }
}
