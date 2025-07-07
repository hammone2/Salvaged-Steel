using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    private bool isMoving;
    private float attackDistance = 150f;

    public int numberOfPoints = 8;       // Number of points to generate

    public float damage = 25f;
    public float bulletSpeed = 10f;
    public Transform bulletSpawner;
    public GameObject bulletPrefab;

    private enum State
    {
        MOVE,
        SHOOT
    }
    private State state;

    public float shootCooldown = 1f;
    private float currentShootCooldown;

    public float attackTime = 10f;
    private float currentAttackTime;

    private float destinationCooldown = 1f;
    private float currentDestinationCooldown;

    public float moveTime = 10f;
    private float currentMoveTime;

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

                currentMoveTime = Mathf.MoveTowards(currentMoveTime, 0f, Time.deltaTime);
                if (currentMoveTime <= 0)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                    isMoving = false;
                    currentAttackTime = attackTime;
                    state = State.SHOOT;
                }
                    
                break;
            
            case State.SHOOT:
                currentShootCooldown = Mathf.MoveTowards(currentShootCooldown, 0f, Time.deltaTime);
                if (currentShootCooldown <= 0)
                    Shoot();

                //if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) > attackDistance)
                currentAttackTime = Mathf.MoveTowards(currentAttackTime, 0f, Time.deltaTime);
                if (currentAttackTime <= 0)
                {
                    agent.isStopped = false;
                    currentDestinationCooldown = 0f;
                    currentMoveTime = moveTime;
                    state = State.MOVE;
                }
                    
                break;
        }
    }

    private void Shoot()
    {
        animator.SetTrigger("TriShoot");
        currentShootCooldown = shootCooldown;

        float radius = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        //use for loop to generate end points for missiles and shoot
        for (int i = 0; i < numberOfPoints; i++)
        {
            //use vector3's here instead of objs
            float angle = i * Mathf.PI * 2f / numberOfPoints;
            Vector3 newPos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            newPos += transform.position;

            GenerateParabola(newPos);
        }

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

    void GenerateParabola(Vector3 endPoint)
    {
        List<Vector3> parabolaPoints = new List<Vector3>();

        float t = Vector3.Distance(transform.position, endPoint); //distance;
        float b = 50f; //height
        float c = t / 2f;
        float a = b / (c * c);

        Vector3 direction = (endPoint - transform.position).normalized;
        float step = 0.5f;
        int numSteps = Mathf.CeilToInt(t / step);

        for (int i = 0; i <= numSteps; i++)
        {
            float x = i * step;
            float y = -a * Mathf.Pow((x - c), 2) + b;
            Vector3 point = transform.position + direction * x + Vector3.up * y;
            parabolaPoints.Add(point);
        }

        //spawn projectile here then clear the points
        if (parabolaPoints.Count < 2) return;

        // Spawn projectile at the starting point
        GameObject projectile = Instantiate(bulletPrefab, bulletSpawner.position, Quaternion.identity);
        List<Vector3> parabolaPointsCopy = new List<Vector3>(parabolaPoints);

        // Start moving the projectile
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.Initialize(parabolaPointsCopy, bulletSpeed, damage);



        parabolaPoints.Clear();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
