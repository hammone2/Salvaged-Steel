using System.Collections;
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

    private void Start()
    {
        state = State.MOVE;
        agent.SetDestination(GameManager.instance.player.transform.position);
        StartCoroutine(SetDestination());
    }

    private void Update()
    {
        switch (state)
        {
            case State.MOVE:
                if (agent.velocity.magnitude > 0.1f)
                    WalkAnimation();

                if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= attackDistance)
                {
                    agent.isStopped = true;
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

    IEnumerator SetDestination()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }
}
