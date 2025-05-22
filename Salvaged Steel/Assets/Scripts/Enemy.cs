using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent agent;
    public AiSensor sensor;
    public GameObject explosionParticles;
    public GameObject rotated;
    public Gun gun;
    public Propulsion propulsion;
    public LayerMask layersToHit;
    public HealthComponent healthComponent;
    public HeaderInfo headerInfo;
    public TextMeshProUGUI stateText;
    public string enemyName;
    public List<GameObject> partSlots;
    public int pointsForKill = 10;

    private int curAttackerId;

    private float rotationSpeed = 8.0f;
    private float propRotSpeed = 8.0f;
    public float detectionDistance = 20f;
    public float playerDetectRate = 0.2f;
    private float lastPlayerDetectTime;
    private float flankWaitTime;
    private float lastIdleTime;
    private float health = 0f; //this isnt the actual health value, only gets passed to the healthcomponent once its calculated
    private LayerMask playerLayer;

    private float minFlankTime = 1f;
    private float maxFlankTime = 5f;
    private float flankRadius = 15f;
    private Transform target;
    private Vector3 lastSpottedPos;
    private GameObject propulsionSlot;

    //burst fire stuff
    private bool canShoot = true;
    private float shootTime;
    private float lastBurstTime;
    public float burstInterval = 3f;

    //Enemy States
    private enum State{
        IDLE,
        PATROL,
        ATTACK,
        SEARCH
    }
    State state;
    
    void Awake()
    {
        agent.speed = propulsion.moveSpeed;
        sensor.distance = detectionDistance;
        playerLayer = LayerMask.GetMask("Player");
    }

    void Start()
    {
        //Apply health to the enemy
        // Loop through each GameObject in the list
        foreach (GameObject obj in partSlots)
        {
            if (obj.name == "PropulsionSlot")
                propulsionSlot = obj;
            // Loop through each direct child of the current GameObject
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);

                // Check if the child has a PartObject component
                PartObject partObject = child.GetComponent<PartObject>();

                if (partObject != null)
                {
                    HealthComponent _healthComponent = partObject.GetComponent<HealthComponent>();
                    if (_healthComponent != null)
                        health += _healthComponent.health;
                }
            }
        }
        //healthComponent.health = health;
        headerInfo.Initialize(enemyName, health);

        ChangeState(State.PATROL);
    }
    void Update()
    {
        switch (state)
        {
            case State.IDLE:
                stateText.text = "Idle";
                if (Time.time - lastIdleTime > 2)
                {
                    lastIdleTime = Time.time;
                    ChangeState(State.PATROL);
                }
                target = null;
                DetectPlayer();
                break;

            case State.PATROL:
                stateText.text = "Patrol";
                ChooseRandomPatrolPosition();
                DetectPlayer();
                break;

            case State.ATTACK:
                stateText.text = "Attack";
                PlayerController player = GameManager.instance.player;
                target = player.transform;

                flankWaitTime -= Time.deltaTime;
                if (flankWaitTime < 0)
                {
                    flankWaitTime = Random.Range(minFlankTime, maxFlankTime);
                    ChooseRandomFlankPosition();
                }

                if (sensor.IsInSight(player.gameObject))
                {
                    if (canShoot)
                    {
                        gun.Shoot(0, false);
                        shootTime -= Time.deltaTime;
                        if (shootTime < 0)
                        {
                            StartCoroutine(BurstFireCooldown());
                        }
                    }
                }

                if (!sensor.IsInRange(player.gameObject))
                {
                    target = player.transform; //null;
                    lastSpottedPos = player.transform.position;
                    agent.SetDestination(lastSpottedPos);
                    ChangeState(State.SEARCH);
                }
                break;

            case State.SEARCH:
                stateText.text = "Search";
                DetectPlayer();
                if (Vector2.Distance(transform.position, lastSpottedPos) < 3)
                {
                    target = null;
                    ChangeState(State.PATROL);
                }
                    
                break;
        }

        //Rotate the turret
        if (target != null)
        {
            Vector3 directionToPlayer = target.position - rotated.transform.position;
            directionToPlayer.y = 0; // Keep the rotation flat
            Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
            rotated.transform.rotation = Quaternion.Slerp(rotated.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

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
        }
    }

    // updates the targeted player
    void DetectPlayer()
    {
        if (Time.time - lastPlayerDetectTime > playerDetectRate)
        {
            lastPlayerDetectTime = Time.time;
            if (GameManager.instance.player != null)
            {
                if (sensor.IsInRange(GameManager.instance.player.gameObject))
                    state = State.ATTACK;
            }
        }
    }
    
    private void ChooseRandomFlankPosition()
    {
        // Get a random position within flankRadius
        Vector3 randomDirection = Random.insideUnitSphere * flankRadius;
        randomDirection.y = 0; // Keep the Y position flat

        // Calculate the target position relative to the player
        if (target != null)
        {
            Vector3 flankPosition = target.position + randomDirection;

            // Set the new destination
            agent.SetDestination(flankPosition);
        }
    }

    private void ChooseRandomPatrolPosition()
    {
        if (target != null)
        {
            if (Vector2.Distance(transform.position, target.position) > 3) //dont get a new waypoint if still traveling
                return;
            else
            {
                //choose a new patrol path or idle
                State[] states = { State.IDLE, State.PATROL }; 
                int randState = Random.Range(0, states.Length);
                ChangeState(states[randState]);
            }
        }

        int destination = Random.Range(0, GameManager.instance.spawnPointList.Count); //using spawnpoints as patrol waypoints so I dont have to get a random point in the world which would result in weird behaviour
        target = GameManager.instance.spawnPointList[destination];
        agent.SetDestination(target.position);
    }

    public void TakeDamage(int attackerId, float damage)
    {
        if (health <= 0)
            return;
        health -= damage;
        headerInfo.UpdateHealthBar(health);
        curAttackerId = attackerId;
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        // Loop through each GameObject in the list
        foreach (GameObject obj in partSlots)
        {
            // Loop through each direct child of the current GameObject
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);

                // Check if the child has a PartObject component
                PartObject partObject = child.GetComponent<PartObject>();

                if (partObject != null)
                {
                    gun = null;
                    partObject.Drop(true, Random.Range(10f, 15f));
                }
            }
        }
        GameManager.instance.GetPlayer().AddKill(pointsForKill);
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        GameManager.instance.enemies--;
        Destroy(gameObject);
    }

    private void ChangeState(State newState)
    {
        agent.isStopped = false; //removed the idle state check to fix permanent stop glicth
        if (newState == State.IDLE)
        {
            agent.isStopped = true; //stop moving
            lastIdleTime = Time.time;
        }
        if (state == State.ATTACK)
        {
            shootTime = burstInterval;
            canShoot = true;
        }

        state = newState;
    }

    IEnumerator BurstFireCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(2);
        shootTime = burstInterval;
        canShoot = true;
    }
}
