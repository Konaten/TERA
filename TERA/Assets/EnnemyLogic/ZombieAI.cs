using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ZombieAI : MonoBehaviour{

    private enum State {
        IdleState,
        WanderState,
        ChaseState,
        AttackState,
        DeadState
    }

    public float health = 100;
    public int argent = 100;

    [SerializeField] private State currentState;

    [Header("IdleState")]
    public float minRestDuration = 2f;
    public float maxRestDuration = 8f;
    private float restDuration;
    private float restTime = 0f;

    [Header("WanderState")]
    public float wanderRange = 5f;
    public float wanderSpeed = 1f;
    private Vector3 wanderPoint;
    private bool wanderPointSet = false;
    
    public float playerDetectionRange = 10f;
    
    [Header("ChaseState")]
    public float chaseRange = 15f;
    public float chaseSpeed = 2f;

    [Header("AttackState")]
    public float attackRange = 1f;
    public float attackCoolDown = 1f;
    public int Degats = 20;
    bool canAttack = true;

    [Header("DeadState")]
    public float timeBeforeDestroy = 3f;
    public bool isDead = false;

    [Header("References")]
    public NavMeshAgent navMeshAgent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    // public WaveZombieSpawner spawner;
    public Animator animator;

    private void Start(){
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentState = State.IdleState;
    }

    private void Update(){
        switch(currentState){
            case State.IdleState : 
                Idle();
                animator.SetBool("isWalking",false);
                animator.SetBool("isRunning",false);
            break;
            case State.WanderState : 
                Wander();
                animator.SetBool("isWalking",true);
                animator.SetBool("isRunning",false);
            break;
            case State.ChaseState : 
                Chase();
                animator.SetBool("isWalking",true);
                animator.SetBool("isRunning",true);
            break;
            case State.AttackState : 
                Attack();
                animator.SetBool("isWalking",false);
                animator.SetBool("isRunning",false);
            break;
            case State.DeadState :
                Dead();
            break;
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            TakeDamage(10);
        }
    }

    private void Idle(){
        if(Physics.CheckSphere(transform.position,playerDetectionRange,whatIsPlayer)){
            ResetIdle();
            currentState = State.ChaseState;
            return;
        }

        if(restTime >= restDuration){
            ResetIdle();
            currentState = State.WanderState;
            return;
        }

        restTime += Time.deltaTime;
    }

    private void Wander(){
        if(Physics.CheckSphere(transform.position,playerDetectionRange,whatIsPlayer)){
            ResetIdle();
            currentState = State.ChaseState;
            return;
        }
        if (!wanderPointSet) SearchWanderPoint();
        
        navMeshAgent.speed = wanderSpeed;

        if (wanderPointSet)
            navMeshAgent.SetDestination(wanderPoint);

        float distanceToWanderPoint = (transform.position - wanderPoint).magnitude;

        if (distanceToWanderPoint < 0.1f){
            wanderPointSet = false;
            ResetWander();
            currentState = State.IdleState;
        }
    }

    private void Chase(){
        navMeshAgent.speed = chaseSpeed;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        navMeshAgent.SetDestination(player.position);
        
        if(distanceToPlayer > chaseRange){    
            ResetWander();
            currentState = State.IdleState;
        }
        else if(distanceToPlayer < attackRange){
            ResetWander();
            currentState = State.AttackState;
        }
    }

    private void Attack(){
        float distanceToPlayer = (transform.position - player.position).magnitude;
        if(!canAttack){
            if(distanceToPlayer > chaseRange){    
                currentState = State.IdleState;
            }
            else if(distanceToPlayer > attackRange){    
                currentState = State.ChaseState;
            }
        }

        transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z));
        
        if(canAttack){
            animator.SetTrigger("isAttacking");   
            // ATTACK IN THE ANIMATION
            canAttack = false;
            Invoke(nameof(ResetAttack),attackCoolDown);
        }
    }

    private void Dead(){
        navMeshAgent.enabled = false;
        animator.enabled = false;
    }

    // RESET
    private void ResetIdle(){
        restDuration = Random.Range(minRestDuration,maxRestDuration);
        restTime = 0f;
    }
    private void ResetWander(){
        navMeshAgent.speed = 0;
        wanderPointSet = false;
    }
    private void ResetAttack(){
        canAttack = true;
        animator.ResetTrigger("isAttacking");   
    }


    // TOOLS
    private void SearchWanderPoint(){
        float randomZ = Random.Range(-wanderRange, wanderRange);
        float randomX = Random.Range(-wanderRange, wanderRange);
        
        wanderPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(wanderPoint, -transform.up, 2f, whatIsGround))
            wanderPointSet = true;
    }

    public void TakeDamage(int damage){
        health -= damage;
        //animator.SetTrigger("isDamaged");
        
        if (health <= 0 && !isDead) DestroyZombie();
    }

    private void DestroyZombie(){
        // player.gameObject.GetComponent<RessourcePlayer>().gagner_argent(argent);
        // isDead = true;
        // currentState = State.DeadState;
        // if(spawner != null){
        //     spawner.EliminateZombie();
        // }
        // Destroy(gameObject,timeBeforeDestroy);
        Debug.Log("destroyzombie");
    }

    public void Attacking(){
        // if(Vector3.Distance(player.position, transform.position) < attackRange){
        //     player.gameObject.GetComponent<RessourcePlayer>().perdre_vie(Degats);
        // }
         Debug.Log("attackzombie");
    }
}
