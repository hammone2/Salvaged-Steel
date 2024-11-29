using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [Header ("Components & Layers")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask partMask;
    public GameObject rotated;
    public CharacterController characterController;
    public Camera playerCamera;

    [Header ("Networking")]
    public Player photonPlayer;
    public int id;
    private int curAttackerId;

    [Header("Stats")]
    public int score;
    private float moveSpeed;
    private float rotationSpeed = 8.0f;
    private float propRotSpeed = 8.0f;
    private float cameraSmoothSpeed = 7.5f;
    private float maxCameraDistance = 7.0f;
    private bool isAlive = true;
    private bool isPlaying = true;

    [Header ("Item Slots")]
    [HideInInspector] public Gun gun;
    [HideInInspector] public Turret turret;
    [HideInInspector] public Propulsion propulsion;
    public GameObject gunSlot;
    public GameObject turretSlot;
    public GameObject propulsionSlot;

    private PartObject selectedPart;
    private int maxPickupDist = 7;

   

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        Debug.Log(id);
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
            HUD.instance.Initialize(this);
        }
    }

    private void Start()
    {
        //Assign parts to vars
        gun = gunSlot.transform.GetChild(0).GetComponent<Gun>();
        SetCustomCursor(gun.crosshair);
        turret = turretSlot.transform.GetChild(0).GetComponent<Turret>();
        rotationSpeed = turret.rotationSpeed;
        propulsion = propulsionSlot.transform.GetChild(0).GetComponent<Propulsion>();
        moveSpeed = propulsion.moveSpeed;
        HUD.instance.InitializeValues();
    }

    private void Update()
    {
        if (!photonView.IsMine) //dont do anything if the photon view isnt the local player's
            return;
        if (!isPlaying) //dont do anything id controls are disabled
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
                    gun.photonView.RPC("DisconnectCamera", RpcTarget.All);
                    gun.gameObject.GetComponent<PartObject>().photonView.RPC("Drop", RpcTarget.All, false, dropForce);
                    selectedPart.Equip(gunSlot.transform, gunSlot.GetComponent<PhotonView>().ViewID);
                    //selectedPart.photonView.RPC("Equip", RpcTarget.Others, gunSlot.transform);
                    gun = selectedPart.GetComponent<Gun>();
                    gun.GetCamera(playerCamera);
                    SetCustomCursor(gun.crosshair);
                    HUD.instance.UpdateAmmoText();
                }
                else if (selectedPart.GetComponent<Turret>())
                {
                    turret.gameObject.GetComponent<PartObject>().photonView.RPC("Drop", RpcTarget.All, false, dropForce);
                    selectedPart.Equip(turretSlot.transform, turretSlot.GetComponent<PhotonView>().ViewID);
                    //selectedPart.photonView.RPC("Equip", RpcTarget.Others, turretSlot.transform);
                    turret = selectedPart.GetComponent<Turret>();
                    HUD.instance.UpdateTurretHealth();
                }
                else if (selectedPart.GetComponent<Propulsion>())
                {
                    propulsion.gameObject.GetComponent<PartObject>().photonView.RPC("Drop", RpcTarget.All, false, dropForce);
                    selectedPart.Equip(propulsionSlot.transform, propulsionSlot.GetComponent<PhotonView>().ViewID);
                    //selectedPart.photonView.RPC("Equip", RpcTarget.Others, propulsionSlot.transform);
                    propulsion = selectedPart.GetComponent<Propulsion>();
                    moveSpeed = propulsion.moveSpeed;
                    HUD.instance.UpdateHullHealth();
                }
            }
        }

        //Shoot
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //gun.Shoot(id, photonView.IsMine);
            gun.photonView.RPC("Shoot", RpcTarget.All, id, photonView.IsMine);
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

    [PunRPC]
    public void TakeDamage(int attackerID, float damage)
    {
        
        if (isAlive == false)
            return;
        HealthComponent propHp = propulsion.GetComponent<HealthComponent>();
        HealthComponent turretHp = turret.GetComponent<HealthComponent>();
        curAttackerId = attackerID;

        float damageFactor = 60f;
        propHp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage / damageFactor);
        turretHp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage / damageFactor);

        HUD.instance.UpdateHullHealth();
        HUD.instance.UpdateTurretHealth();

        if (propHp.health <= 0 || turretHp.health <= 0)
        {
            photonView.RPC("Die", RpcTarget.All);
        }
        
    }

    [PunRPC]
    public void Die()
    {
        isAlive = false;
        End();
    }

    [PunRPC]
    public void AddKill(int scoreToAdd)
    {
        score += scoreToAdd;
        HUD.instance.UpdateScoreText();
    }

    public void End()
    {
        isPlaying = false;
        //playButton.SetActive(true);
        Leaderboard.instance.SetLeaderboardEntry(score);
    }

    public void Begin()
    {
        isPlaying = true;
        //playButton.SetActive(false);
    }

    private void SetCustomCursor(Texture2D cursorTexture)
    {
        Vector2 cursorHotspot = new Vector2(cursorTexture.width/2, cursorTexture.height/2); // get the center of the crosshair
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}
