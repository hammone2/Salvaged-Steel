using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{

    public NavMeshAgent agent;

    public GameObject rotated;
    public CharacterController characterController;
    public Gun gun;
    public Propulsion propulsion;
    public LayerMask layersToHit;
    public HealthComponent healthComponent;
    public List<GameObject> partSlots;
    public int pointsForKill = 10;

    private int curAttackerId;

    private float rotationSpeed = 8.0f;
    private float propRotSpeed = 8.0f;
    private float detectionDistance = 20f;
    private float chaseRange = 60f;
    public float playerDetectRate = 0.2f;
    private float lastPlayerDetectTime;
    private float health = 0f; //this isnt the actual health value, only gets passed to the healthcomponent once its calculated
    private LayerMask playerLayer;

    private float minFlankTime = 1f;
    private float maxFlankTime = 5f;
    private float flankRadius = 15f;
    private Transform target;
    private GameObject propulsionSlot;
    
    void Awake()
    {
        agent.speed = propulsion.moveSpeed;
        playerLayer = LayerMask.GetMask("Player");
    }

    void Start()
    {
        //GameObject playerObject = GameObject.FindWithTag("Player");
        //target = playerObject.transform;

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
                    Debug.Log("Found PartObject in child: " + child.name);
                    HealthComponent _healthComponent = partObject.GetComponent<HealthComponent>();
                    if (_healthComponent != null)
                        health += _healthComponent.health;
                }
            }
        }
        //healthComponent.health = health;

        // Start the coroutine to choose random positions
        StartCoroutine(ChooseRandomFlankPosition());

        
    }
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (target != null)
        {
            Vector3 directionToPlayer = target.position - rotated.transform.position;
            directionToPlayer.y = 0; // Keep the rotation flat
            Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
            rotated.transform.rotation = Quaternion.Slerp(rotated.transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            // Raycast from rotated to detect the player
            RaycastHit hit;
            if (Physics.Raycast(rotated.transform.position, rotated.transform.forward, out hit, detectionDistance, layersToHit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //gun.Shoot(0, false);
                    gun.photonView.RPC("Shoot", RpcTarget.All, 0, false);
                }
            }
        }

        //Search for nearby players
        DetectPlayer();

        //rotate the propulsion part
        // Get the current velocity of the agent (NavMeshAgent)
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
            // loop through all the players
            foreach (PlayerController player in GameManager.instance.players)
            {
                // calculate distance between us and the player
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (player == target)
                {
                    if (dist > chaseRange)
                        target = null;
                }
                else if (dist < chaseRange)
                {
                    if (target == null)
                        target = player.transform;
                }
            }
        }
    }
    
    private IEnumerator ChooseRandomFlankPosition()
    {
        while (true)
        {
            // Wait for a random time interval
            float waitTime = Random.Range(minFlankTime, maxFlankTime);
            yield return new WaitForSeconds(waitTime);

            // Get a random position within flankRadius
            Vector3 randomDirection = Random.insideUnitSphere * flankRadius;
            randomDirection.y = 0; // Keep the Y position flat

            // Calculate the target position relative to the player
            Vector3 flankPosition = target.position + randomDirection;

            // Set the new destination
            agent.SetDestination(flankPosition);
            //target.position = flankPosition;

            // Wait until the agent has reached the new destination
            /*while (agent.remainingDistance > closeEnoughDistance)
            {
                yield return null; // Wait until next frame
            }*/
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, float damage)
    {
        if (health <= 0)
            return;
        health -= damage;
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
                    Debug.Log("Found PartObject in child: " + child.name);
                    gun = null;
                    partObject.Drop(true, Random.Range(10f, 15f));
                }
            }
        }
        if (curAttackerId != 0)
            GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All, pointsForKill);
        PhotonNetwork.Destroy(gameObject);
    }
}
