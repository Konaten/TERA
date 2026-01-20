using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ZombieAI : MonoBehaviour
{

    private enum State
    {
        IdleState,
        WanderState,
        ChaseState,
        AttackState,
        DeadState
    }

    public float health = 100;
    public float maxHealth;
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
    public float damage = 20;
    bool canAttack = true;

    [Header("DeadState")]
    public float timeBeforeDestroy = 3f;
    public bool isDead = false;
    [Header("Effets")]
    public GameObject bloodEffectPrefab;

    [Header("References")]
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private Joueur playerScript;
    public LayerMask whatIsGround;
    private Animator animator;
    public ZombieSpawner spawner;
    public Slider healthBar;

    private Rigidbody[] allRigidbodies;
    private Collider[] allColliders;
    private Rigidbody rootRigidbody;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rootRigidbody = GetComponent<Rigidbody>();

        allRigidbodies = GetComponentsInChildren<Rigidbody>();
        allColliders = GetComponentsInChildren<Collider>();

        SetRagdollState(false);

        currentState = State.IdleState;

        if (Camera.main != null)
        {
            player = Camera.main.transform;    
            playerScript = FindObjectOfType<Joueur>();

            if (playerScript == null)
            {
                Debug.LogError("Erreur : Joueur script non trouvé sur XR Origin !");
            }
        }
        else
        {
            Debug.LogError("Erreur : Impossible de trouver la Main Camera (le casque VR) !");
        }

        maxHealth = health;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.IdleState:
                Idle();
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                break;
            case State.WanderState:
                Wander();
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                break;
            case State.ChaseState:
                Chase();
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", true);
                break;
            case State.AttackState:
                Attack();
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                break;
            case State.DeadState:
                Dead();
                break;
        }

        if(Input.GetKeyDown(KeyCode.P)){
            Dead();
        }
    }

    private void Idle()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < playerDetectionRange)
        {
            ResetIdle();
            currentState = State.ChaseState;
            return;
        }

        if (restTime >= restDuration)
        {
            ResetIdle();
            currentState = State.WanderState;
            return;
        }

        restTime += Time.deltaTime;
    }

    private void Wander()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < playerDetectionRange)
        {
            ResetIdle();
            currentState = State.ChaseState;
            return;
        }

        if (!wanderPointSet) SearchWanderPoint();

        navMeshAgent.speed = wanderSpeed;

        if (wanderPointSet)
            navMeshAgent.SetDestination(wanderPoint);

        float distanceToWanderPoint = (transform.position - wanderPoint).magnitude;

        if (distanceToWanderPoint < 0.1f)
        {
            wanderPointSet = false;
            ResetWander();
            currentState = State.IdleState;
        }
    }

    private void Chase()
    {
        navMeshAgent.speed = chaseSpeed;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        navMeshAgent.SetDestination(player.position);

        if (distanceToPlayer > chaseRange)
        {
            ResetWander();
            currentState = State.IdleState;
        }
        else if (distanceToPlayer < attackRange)
        {
            ResetWander();
            currentState = State.AttackState;
        }
    }

    private void Attack()
    {
        float distanceToPlayer = (transform.position - player.position).magnitude;
        if (!canAttack)
        {
            if (distanceToPlayer > chaseRange)
            {
                currentState = State.IdleState;
            }
            else if (distanceToPlayer > attackRange)
            {
                currentState = State.ChaseState;
            }
        }

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (canAttack)
        {
            animator.SetTrigger("isAttacking");
            // ATTACK IN THE ANIMATION
            canAttack = false;
            Invoke(nameof(ResetAttack), attackCoolDown);
            playerScript.PvRemoved(damage);
        }
    }

    private void Dead()
    {
        if (isDead) return;

        isDead = true;
        currentState = State.DeadState;

        if (navMeshAgent != null) navMeshAgent.enabled = false;

        navMeshAgent.enabled = false;
        animator.enabled = false;

        SetRagdollState(true);

        DestroyZombie();
    }

    // RESET
    private void ResetIdle()
    {
        restDuration = Random.Range(minRestDuration, maxRestDuration);
        restTime = 0f;
    }
    private void ResetWander()
    {
        navMeshAgent.speed = 0;
        wanderPointSet = false;
    }
    private void ResetAttack()
    {
        canAttack = true;
        animator.ResetTrigger("isAttacking");
    }


    // TOOLS
    private void SearchWanderPoint()
    {
        float randomZ = Random.Range(-wanderRange, wanderRange);
        float randomX = Random.Range(-wanderRange, wanderRange);

        wanderPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(wanderPoint, -transform.up, 2f, whatIsGround))
            wanderPointSet = true;
    }

    public void TakeDamage(float damage, RaycastHit hitInfo)
    {
        health -= damage;
        if (bloodEffectPrefab != null)
        {
            SpawnBlood(hitInfo);
        }
        //animator.SetTrigger("isDamaged");

        if (healthBar != null)
        {
            healthBar.value = health;
        }

        if (health <= 0 && !isDead) Dead();
    }

    void SpawnBlood(RaycastHit hit)
    {
        GameObject blood = Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        blood.transform.SetParent(hit.transform);
        blood.transform.localPosition += Vector3.forward * 0.01f;
        float randomScale = Random.Range(0.03f, 0.05f);
        blood.transform.localScale = new Vector3(randomScale, randomScale, 1);
        Destroy(blood, 10f);
    }

    private void DestroyZombie()
    {
        isDead = true;
        currentState = State.DeadState;

        playerScript.AjouterArgent(argent);

        if (spawner != null)
        {
            spawner.OnZombieKilled();
        }

        Destroy(gameObject, timeBeforeDestroy);
    }

    public void Attacking()
    {
        // if(Vector3.Distance(player.position, transform.position) < attackRange){
        //     player.gameObject.GetComponent<RessourcePlayer>().perdre_vie(Degats);
        // }
        // Debug.Log("attackzombie");
    }

    private void SetRagdollState(bool active)
    {
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb == rootRigidbody) 
            {
                rb.isKinematic = true; 
                continue;
            }
            
            rb.isKinematic = !active;
            rb.useGravity = active;
        }

        foreach (Collider col in allColliders)
        {
            col.enabled = true;
        }
    }
}
