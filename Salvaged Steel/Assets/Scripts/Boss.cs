using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    private bool isMoving;
    private float attackDistance = 10f;

    private enum State
    {
        MOVE,
        SHOOT
    }
    private State state;

    public float shootCooldown = 1f;
    private float currentShootCooldown;

    private float destinationCooldown = 1f;
    private float currentDestinationCooldown;

    private void Start()
    {
        state = State.MOVE;
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    private void Update()
    {
        switch (state)
        {
            case State.MOVE:
                if (agent.velocity.magnitude > 0.1f)
                    WalkAnimation();

                currentDestinationCooldown = Mathf.MoveTowards(currentDestinationCooldown, 0f, Time.deltaTime);
                if (currentDestinationCooldown <= 0)
                    SetDestination();


                if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= attackDistance)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                    isMoving = false;
                    state = State.SHOOT;
                }
                    
                break;
            
            case State.SHOOT:
                currentShootCooldown = Mathf.MoveTowards(currentShootCooldown, 0f, Time.deltaTime);
                if (currentShootCooldown <= 0)
                    Shoot();

                if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) > attackDistance)
                {
                    agent.isStopped = false;
                    currentDestinationCooldown = 0f;
                    state = State.MOVE;
                }
                    
                break;
        }
    }

    private void Shoot()
    {
        animator.SetTrigger("TriShoot");
        currentShootCooldown = shootCooldown;
    }

    private void WalkAnimation()
    {
        if (isMoving)
            return;
        isMoving = true;
        animator.SetTrigger("TriWalk");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
            Destroy(other.gameObject);
    }

    private void SetDestination()
    {
        currentDestinationCooldown = destinationCooldown;
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
