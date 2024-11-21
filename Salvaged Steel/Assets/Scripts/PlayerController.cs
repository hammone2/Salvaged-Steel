using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    //components, objects, etc
    [Header ("Components & Layers")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask partMask;
    public GameObject rotated;
    public GameObject playButton;
    public CharacterController characterController;
    public Camera playerCamera;

    [Header ("Networking")]
    //networking stuff
    public Player photonPlayer;
    public int id;

    [Header ("Stats")]
    //stats
    private float moveSpeed;
    private float rotationSpeed = 8.0f;
    private float propRotSpeed = 8.0f;
    private float cameraSmoothSpeed = 7.5f;
    private float maxCameraDistance = 7.0f;
    private float health = 10f;
    private bool isAlive = true;
    private bool isPlaying;

    [Header ("Item Slots")]
    //item slots
    [HideInInspector] public Gun gun;
    [HideInInspector] public Turret turret;
    [HideInInspector] public Propulsion propulsion;
    public GameObject gunSlot;
    public GameObject turretSlot;
    public GameObject propulsionSlot;

    private PartObject selectedPart;
    private int maxPickupDist = 7;

    private void Start()
    {
        //Assign parts to vars
        gun = gunSlot.transform.GetChild(0).GetComponent<Gun>();
        SetCustomCursor(gun.crosshair);
        turret = turretSlot.transform.GetChild(0).GetComponent<Turret>();
        rotationSpeed = turret.rotationSpeed;
        propulsion = propulsionSlot.transform.GetChild(0).GetComponent<Propulsion>();
        moveSpeed = propulsion.moveSpeed;
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;
        // is this not our local player?
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            //rig.isKinematic = true;
        }
        else
        {
            //GameUI.instance.Initialize(this);
        }
    }

    private void Update()
    {
        if (!isPlaying)
            return;
        if (!isAlive) // Dont do anything if the player is dead
            return;

        //Move
            // Get input for movement (WASD or arrow keys)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
            // Apply movement and normalize the vector using ClampedMagnitude()
        Vector3 move = new Vector3(horizontal, 0, vertical);
        characterController.Move(Vector3.ClampMagnitude(move, 1.0f) * Time.deltaTime * moveSpeed);
        // If there is movement, rotate propulsionSlot to follow move direction
        if (move.magnitude > 0.1f)
        {
            // Calculate the direction vector (ignoring the Y axis)
            Vector3 moveDirection = move.normalized;

            // Calculate the target rotation, rotating only around the Y axis (since we're on a flat plane)
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // Smoothly rotate the propulsionSlot towards the target rotation
            propulsionSlot.transform.rotation = Quaternion.Lerp(propulsionSlot.transform.rotation, targetRotation, Time.deltaTime * propRotSpeed);
        }

        //Aim
        Aim();

        //Picking up parts off the ground
        LookForParts();
        if (selectedPart != null)
        {
            float distance = Vector3.Distance(transform.position, selectedPart.transform.position);
            if (Input.GetKeyDown(KeyCode.E) && selectedPart.isEquipped == false && distance <= maxPickupDist) 
            {
                float dropForce = 7f;
                if (selectedPart.GetComponent<Gun>())
                {
                    gun.gameObject.GetComponent<PartObject>().Drop(false, dropForce);
                    selectedPart.Equip(gunSlot.transform);
                    gun = selectedPart.GetComponent<Gun>();
                    gun.GetCamera(playerCamera);
                    SetCustomCursor(gun.crosshair);
                }
                else if (selectedPart.GetComponent<Turret>())
                {
                    turret.gameObject.GetComponent<PartObject>().Drop(false, dropForce);
                    selectedPart.Equip(turretSlot.transform);
                    turret = selectedPart.GetComponent<Turret>();
                }
                else if (selectedPart.GetComponent<Propulsion>())
                {
                    propulsion.gameObject.GetComponent<PartObject>().Drop(false, dropForce);
                    selectedPart.Equip(propulsionSlot.transform);
                    propulsion = selectedPart.GetComponent<Propulsion>();
                    moveSpeed = propulsion.moveSpeed;
                }
            }
        }

        //Shoot
        if (Input.GetKey(KeyCode.Mouse0))
        {
            gun.Shoot();
        }
    }

    private void LookForParts()
    {
        var ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, partMask))
        {
            PartObject partObject = hitInfo.collider.gameObject.GetComponent<PartObject>();
            if (partObject != null)
            {
                selectedPart = partObject;
            }           
        }
        else
        {
            selectedPart = null;
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            // The Raycast hit something, return with the position.
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // The Raycast did not hit anything.
            return (success: false, position: Vector3.zero);
        }
    }

    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            var direction = position - transform.position;
            direction.y = 0; //So the player aims straight and dosen't look at the floor

            // Calculate the halfway point between the player and the mouse hit position
            Vector3 halfwayPoint = (transform.position + position) / 2;

            // Clamp the halfway point to be within a certain radius around the player
            Vector3 directionToHalfway = halfwayPoint - transform.position;
            if (directionToHalfway.magnitude > maxCameraDistance)
            {
                halfwayPoint = transform.position + directionToHalfway.normalized * maxCameraDistance;
            }

            // Lerp camera position towards the halfway point
            halfwayPoint.y = 10.5f; //doing this so the camera dosen't clip into the ground and cause weird shadows
            halfwayPoint.z += -18.66f;
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, halfwayPoint, Time.deltaTime * cameraSmoothSpeed);

            // Smooth rotation towards the target direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rotated.transform.rotation = Quaternion.Slerp(rotated.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); //rotated.transform.forward = direction;
        }
    }

    public void TakeDamage(float damage)
    {
        
        if (isAlive == false)
            return;
        HealthComponent propHp = propulsion.GetComponent<HealthComponent>();
        HealthComponent turretHp = turret.GetComponent<HealthComponent>();

        float damageFactor = 30f;
        propHp.TakeDamage(damage / damageFactor);
        turretHp.TakeDamage(damage / damageFactor);

        if (propHp.health <= 0 || turretHp.health <= 0)
        {
            isAlive = false;
            End(); //get rid of this later when implementing multiplayer
        }
        
    }

    public void End()
    {
        isPlaying = false;
        playButton.SetActive(true);
        Leaderboard.instance.SetLeaderboardEntry(Mathf.RoundToInt(Global.score));
    }

    public void Begin()
    {
        isPlaying = true;
        playButton.SetActive(false);
    }

    private void SetCustomCursor(Texture2D cursorTexture)
    {
        Vector2 cursorHotspot = new Vector2(cursorTexture.width/2, cursorTexture.height/2); // get the center of the crosshair
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}
