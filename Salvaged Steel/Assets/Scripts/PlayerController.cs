using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask partMask;
    public GameObject rotated;
    public CharacterController characterController;
    public Camera playerCamera;

    private float moveSpeed;
    private float rotationSpeed = 8.0f;
    private float propRotSpeed = 8.0f;
    private float cameraSmoothSpeed = 7.5f;
    private float maxCameraDistance = 7.0f;
    private float health = 10f;
    private bool isAlive = true;

    [HideInInspector] public Gun gun;
    private Turret turret;
    private Propulsion propulsion;
    public GameObject gunSlot;
    public GameObject turretSlot;
    public GameObject propulsionSlot;

    private PartObject selectedPart;
    private int maxPickupDist = 7;

    private void Start()
    {
        //Assign parts to vars
        gun = gunSlot.transform.GetChild(0).GetComponent<Gun>();
        turret = turretSlot.transform.GetChild(0).GetComponent<Turret>();
        rotationSpeed = turret.rotationSpeed;
        propulsion = propulsionSlot.transform.GetChild(0).GetComponent<Propulsion>();
        moveSpeed = propulsion.moveSpeed;
    }

    private void Update()
    {
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
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, halfwayPoint, Time.deltaTime * cameraSmoothSpeed);

            // Smooth rotation towards the target direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rotated.transform.rotation = Quaternion.Slerp(rotated.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); //rotated.transform.forward = direction;
        }
    }

    public void TakeDamage(float damage)
    {

    }
}
