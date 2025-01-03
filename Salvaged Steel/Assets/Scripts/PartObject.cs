using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PartObject : MonoBehaviourPun
{
    public Rigidbody rb;
    public BoxCollider bc;
    public ParticleSystem fireParticles;
    public GameObject infoPrefab;
    public bool isEquipped = true;
    [SerializeField] private Outline outline;
    private Transform originalParent;
    private Transform currentParent;
    private Vector3 forceDirection;

    // Timer variables
    private float despawnTime = 45f; // Time in seconds before the item despawns
    private float currentTimer = 0f; // Current time on the timer
    private Coroutine despawnCoroutine; // Reference to the coroutine
    private bool selected = false;


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

    // Dictionary to store rarity-color mappings
    private static readonly Dictionary<Rarity, Color> rarityColors = new Dictionary<Rarity, Color>()
    {
        { Rarity.Common, Color.white },
        { Rarity.Uncommon, Color.green },
        { Rarity.Rare, Color.blue },
        { Rarity.Epic, new Color(0.5f, 0, 0.5f) }, // Purple color
        { Rarity.Legendary, Color.yellow }
    };

    // Method to get color based on the item's rarity
    public Color GetRarityColor()
    {
        return rarityColors[itemRarity];
    }
    
    void Start()
    {
        originalParent = transform.parent; // Store the original parent so that we can reassign it when dropping
        outline.OutlineColor = GetRarityColor(); //set the rarity color
    }

    public void ShowInfo()
    {
        if (isEquipped)
            return;
        if (selected == true)
            return;
        selected = true;
        infoPrefab.SetActive(true);
        infoPrefab.GetComponent<ItemInfo>().Initialize(GetRarityColor(), gameObject);
    }

    public void HideInfo()
    {
        infoPrefab.GetComponent<ItemInfo>().selected = false;
        infoPrefab.SetActive(false);
        selected = false;
    }

    public void Equip(Transform newParent, int parentID) //doing a local call then a RPC call to get around Photon not being able to serialize transforms
    {
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();  // Request ownership if it's not owned by the current player
        }

        // Set the object as a child of the new parent
        currentParent = newParent;
        photonView.RPC("EquipRPC", RpcTarget.All, newParent.position, newParent.rotation, parentID);
    }

    [PunRPC]
    public void EquipRPC(Vector3 parentPosition, Quaternion parentRotation, int parentID)
    {
        fireParticles.Stop();
        outline.enabled = false;
        Debug.Log("EQUIP RPC ATTEMPT");
        isEquipped = true;

        if (PhotonNetwork.IsMasterClient) // do physics stuff if executed on the master client
        {
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
        }

        // Set the object as a child of the new parent
        transform.SetParent(PhotonView.Find(parentID).transform);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    [PunRPC]
    public void Drop(bool isExploding, float forceStrength)
    {
        if (photonView.IsMine)
        {
            photonView.TransferOwnership(0); // Removes ownership (transfers to server)

            // Option 2: If you want to transfer ownership to another player (for example, Player B):
            // photonView.TransferOwnership(otherPlayerId); // Use the player's ID to transfer ownership
        }

        isEquipped = false;
        // Unparent the object
        transform.SetParent(null);
        currentParent = null;

        // Set its world position and rotation (this lets it behave like a world object)
        // You can adjust this if you want it to fall from a specific point, like a character's hand
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z); // Set this to whatever you want

        if (PhotonNetwork.IsMasterClient)
        {
            if (rb != null)
            {
                rb.isKinematic = false;  // Allow it to fall if there's a Rigidbody
                rb.useGravity = true;

                // Apply force to send the object flying
                if (isExploding)
                {
                    forceDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f));
                }
                else
                {
                    forceDirection = transform.forward;
                }

                rb.AddForce(forceDirection.normalized * forceStrength, ForceMode.Impulse);  // You can use ForceMode.Impulse for instant impact
            }
            if (bc != null)
            {
                bc.isTrigger = false; //enable collisions
            }

            despawnCoroutine = StartCoroutine(DespawnTimer());
        }
        
        if (isExploding)
            fireParticles.Play();
        outline.enabled = true;
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
    public void DespawnItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
