using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PartObject : MonoBehaviourPun
{
    public Rigidbody rb;
    public BoxCollider bc;
    public bool isEquipped = true;
    private Transform originalParent;
    private Vector3 forceDirection;

    // Timer variables
    private float despawnTime = 45f; // Time in seconds before the item despawns
    private float currentTimer = 0f; // Current time on the timer
    private Coroutine despawnCoroutine; // Reference to the coroutine


    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    // Public list to hold only one rarity (always contains exactly one item)
    [SerializeField]
    public Rarity itemRarity = Rarity.Common;  // Default value set to Common

    // Store the original parent so that we can reassign it when dropping
    void Start()
    {
        originalParent = transform.parent;
    }

    [PunRPC]
    public void Equip(Transform newParent)
    {
        Debug.Log("EQUIP ATTEMPT");
        isEquipped = true;
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; // stop doing physics stuff
        }
        if (bc != null)
        {
            bc.isTrigger = true; //disable collisions
        }

        //Stop the despawn timer
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            currentTimer = 0f; // Reset the timer
        }

        // Set the object as a child of the new parent
        transform.SetParent(newParent);

        // Position the object at the parent's position and rotation (optional: you could adjust it further)
        transform.localPosition = Vector3.zero;  // Or any custom position relative to the parent
        transform.localRotation = Quaternion.identity;  // Or any custom rotation
    }

    [PunRPC]
    public void Drop(bool isExploding, float forceStrength)
    {
        isEquipped = false;
        // Unparent the object
        transform.SetParent(null);

        // Set its world position and rotation (this lets it behave like a world object)
        // You can adjust this if you want it to fall from a specific point, like a character's hand
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z); // Set this to whatever you want

        if (rb != null)
        {
            rb.isKinematic = false;  // Allow it to fall if there's a Rigidbody
            rb.useGravity = true;

            // Apply force to send the object flying
            if (isExploding)
                forceDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f));
            else
                forceDirection = transform.forward;
      

            rb.AddForce(forceDirection.normalized * forceStrength, ForceMode.Impulse);  // You can use ForceMode.Impulse for instant impact
        }
        if (bc != null)
        {
            bc.isTrigger = false; //enable collisions
        }

        despawnCoroutine = StartCoroutine(DespawnTimer());
    }

    // Coroutine that handles the despawn timer
    private IEnumerator DespawnTimer()
    {
        currentTimer = despawnTime;

        while (currentTimer > 0f)
        {
            currentTimer -= Time.deltaTime; // Decrease the timer over time
            yield return null; // Wait for the next frame
        }

        // When the timer reaches zero, despawn the item (destroy it)
        DespawnItem();
    }

    // Method to destroy or deactivate the item
    private void DespawnItem()
    {
        //Debug.Log("Item despawned: " + name);
        Destroy(gameObject); // Destroy the item GameObject
    }
}
