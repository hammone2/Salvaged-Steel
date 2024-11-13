using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent agent;

    public GameObject rotated;
    public CharacterController characterController;
    public Gun gun;
    public LayerMask layersToHit;
    public HealthComponent healthComponent;
    public List<GameObject> partSlots;

    private float moveSpeed = 7.5f;
    private float rotationSpeed = 8.0f;
    private float detectionDistance = 20f;
    private LayerMask playerLayer;

    private float minFlankTime = 1f;
    private float maxFlankTime = 5f;
    private float closeEnoughDistance = 1f;
    private float flankRadius = 15f;
    private Transform target;
    
    void Awake()
    {
        agent.speed = moveSpeed;
        playerLayer = LayerMask.GetMask("Player");
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        target = playerObject.transform;

        // Start the coroutine to choose random positions
        StartCoroutine(ChooseRandomFlankPosition());

        
    }
    void Update()
    {
        //agent.destination = target.position;

        // If the enemy has reached the destination, continue the flank process
        /*if (agent.remainingDistance <= closeEnoughDistance && !agent.pathPending)
        {
            // Wait for a short time before choosing a new flank position
            StartCoroutine(ChooseRandomFlankPosition());
        }*/

        if (target != null)
        {
            Vector3 directionToPlayer = target.position - rotated.transform.position;
            directionToPlayer.y = 0; // Keep the rotation flat
            Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
            rotated.transform.rotation = Quaternion.Slerp(rotated.transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            // Raycast from rotated to detect the player
            RaycastHit hit;
            if (Physics.Raycast(rotated.transform.position, rotated.transform.forward /*directionToPlayer.normalized*/, out hit, detectionDistance, layersToHit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    gun.Shoot();
                }
                //Debug.Log(hit.collider.gameObject.name + " was hit!");
            }
        }

        if (healthComponent.health <= 0)
        {
            Die();
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
                    partObject.Drop();
                }
            }
        }

        Destroy(this.gameObject);
    }
}
