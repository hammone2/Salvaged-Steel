using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    public GameObject rotated;
    public CharacterController characterController;
    public Camera playerCamera;

    private float moveSpeed = 7.5f;
    private float rotationSpeed = 8.0f;
    private float cameraSmoothSpeed = 7.5f;
    private float maxCameraDistance = 7.0f;
    private bool isAlive = true;

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

        //Aim
        Aim();
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
            //rotated.transform.forward = direction;

            //lerp camera toward mouse position
            //playerCamera.transform.position = position;

            // Smooth rotation towards the target direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rotated.transform.rotation = Quaternion.Slerp(rotated.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Calculate the desired camera position
            Vector3 desiredCameraPosition = position;

            // Ensure the camera does not exceed the max distance radius from the player
            Vector3 directionToPlayer = desiredCameraPosition - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            // If the distance is greater than the max distance, clamp it
            if (distanceToPlayer > maxCameraDistance)
            {
                directionToPlayer = directionToPlayer.normalized * maxCameraDistance;
                desiredCameraPosition = transform.position + directionToPlayer;
            }

            // Smoothly move the camera towards the desired position (now within max distance)
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, desiredCameraPosition, Time.deltaTime * cameraSmoothSpeed);
        }
    }
}
