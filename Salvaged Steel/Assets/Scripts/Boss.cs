using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    private bool isMoving;

    public int numberOfPoints = 8;       // Number of points to generate

    public float damage = 25f;
    public float bulletSpeed = 10f;
    public float turretRotationSpeed = 360f;
    public Transform bulletSpawner;
    public GameObject bulletPrefab;
    public GameObject turret;
    public float gunRange = 300f;
    public Gun gun;

    public LayerMask occlusionLayers;

    public GameObject propulsionSlot;
    public float propRotSpeed = 8f;

    public HeaderInfo headerInfo;
    public HealthComponent[] healthComponents;
    public float health;
    private float currentHealth;

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

    public string enemyName;

    private void Start()
    {
        state = State.MOVE;
        agent.SetDestination(GameManager.instance.player.transform.position);
        headerInfo.Initialize(enemyName, health);

        for (int i = 0; i < healthComponents.Length; i++)
        {
            HealthComponent healthComponent = healthComponents[i];
            healthComponent.health = health;
        }

        currentHealth = health;
    }

    private void Update()
    {
        switch (state)
        {
            case State.MOVE:

                //rotate the propulsion part
                Vector3 velocity = agent.velocity;

                // If the enemy is moving (velocity magnitude > 0), rotate the propulsionSlot
                if (velocity.magnitude > 0.1f)
                {
                    // Get the direction the enemy is moving (ignore Y axis)
                    Vector3 moveDirection = new Vector3(velocity.x, 0, velocity.z).normalized;

                    // Calculate the target rotation, looking in the direction of movement
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                    // Smoothly rotate the propulsionSlot towards the target rotation
                    propulsionSlot.transform.rotation = Quaternion.Lerp(propulsionSlot.transform.rotation, targetRotation, Time.deltaTime * propRotSpeed);

                    WalkAnimation();
                }

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

        //Rotate the turret
        Vector3 playerPos = GameManager.instance.player.transform.position;
        playerPos.y = 0.5f; //doing this so the raycast actually hits the player
        Vector3 directionToPlayer = playerPos - turret.transform.position;
        Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, rotation, Time.deltaTime * turretRotationSpeed);

        //turret raycast
        RaycastHit hit;
        Vector3 forward = turret.transform.TransformDirection(Vector3.forward) * gunRange;
        Debug.DrawRay(turret.transform.position, forward, Color.greenYellow);
        if (Physics.Raycast(turret.transform.position, forward, out hit, gunRange, occlusionLayers))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                gun.Shoot(0, false);
            } 
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

    public void TakeDamage()
    {
        float temp = health;

        for (int i = 0; i < healthComponents.Length; i++)
        {
            HealthComponent healthComponent = healthComponents[i];
            float damage = healthComponent.maxHealth - healthComponent.health;
            temp -= damage;
        }

        currentHealth = temp;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        headerInfo.UpdateHealthBar(currentHealth);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
