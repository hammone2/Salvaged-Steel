using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomLevelGenerator : MonoBehaviour
{
    public GameObject wallPrefab;  // Wall prefab (e.g., cube)
    public GameObject floorPrefab; // Floor prefab (e.g., empty space or plane)
    public int width = 50;         // Grid width (number of tiles in X direction)
    public int height = 50;        // Grid height (number of tiles in Z direction)
    public int walkLength = 500;   // Number of steps the drunkard will take

    public int tileSize = 1;      // Size of each tile (wall/floor) in world space

    private Vector3 currentPos;    // Current position of the drunkard (world position)
    private GameObject[,] grid;    // 2D array to hold grid references (wall/floor objects)
    private bool[,] visited;       // 2D array to track visited tiles (floor or wall)

    void Start()
    {
        // Initialize the grid and set up walls
        InitializeGrid();

        // Start the drunkard's walk from a random position
        currentPos = new Vector3(Random.Range(1, width - 2) * tileSize, 0, Random.Range(1, height - 2) * tileSize);

        // Start the walk process
        StartCoroutine(DrunkardsWalkCoroutine());
    }

    void InitializeGrid()
    {
        grid = new GameObject[width, height];
        visited = new bool[width, height];  // Initialize the visited array

        // Populate the grid with wall prefabs
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize); // Adjust the position based on the tile size
                grid[x, z] = Instantiate(wallPrefab, position, Quaternion.identity);
                visited[x, z] = false;  // Mark all tiles as unvisited initially
            }
        }
    }

    IEnumerator DrunkardsWalkCoroutine()
    {
        // Start carving the path by turning the initial position into a floor
        CarveFloor(currentPos);

        // Walk for the specified number of steps
        //for (int i = 0; i <= walkLength; i++)
        while (walkLength > 0)
        {
            // Randomly choose a direction to walk
            MoveRandomDirection();

            // If the new position is already a floor, skip this step
            if (visited[Mathf.FloorToInt(currentPos.x / tileSize), Mathf.FloorToInt(currentPos.z / tileSize)])
            {
                continue; // Skip to the next iteration if the tile has already been visited
            }

            // Carve the current position into a floor (remove wall)
            CarveFloor(currentPos);

            //Debug.Log("Step #: "+i+" / "+walkLength);
            walkLength--;

            // Wait a small amount of time before the next step
            yield return new WaitForSeconds(0.001f);
        }
    }

    void MoveRandomDirection()
    {
        // Randomly pick a direction (0 - 3: north, south, east, west)
        int direction = Random.Range(0, 4);

        // Calculate the movement step based on tileSize
        Vector3 step = Vector3.zero;

        switch (direction)
        {
            case 0:
                step += new Vector3(tileSize, 0, 0);  // Move right (east)
                break;
            case 1:
                step += new Vector3(-tileSize, 0, 0); // Move left (west)
                break;
            case 2:
                step += new Vector3(0, 0, tileSize);  // Move up (north)
                break;
            case 3:
                step += new Vector3(0, 0, -tileSize); // Move down (south)
                break;
        }

        // Calculate the new position in world space
        Vector3 newPos = currentPos + step;

        // Convert world position to grid position (indices)
        int gridX = Mathf.FloorToInt(newPos.x / tileSize);
        int gridZ = Mathf.FloorToInt(newPos.z / tileSize);

        // Prevent moving into the outermost edges (left, right, bottom, top)
        if (gridX > 0 && gridX < width - 1 && gridZ > 0 && gridZ < height - 1)
        {
            // Update the position only if it's within the valid bounds
            currentPos = newPos;
        }

        // Keep the drunkard within bounds of the grid
        currentPos.x = Mathf.Clamp(currentPos.x, 0, (width - 2) * tileSize);
        currentPos.z = Mathf.Clamp(currentPos.z, 0, (height - 2) * tileSize);
    }

    void CarveFloor(Vector3 position)
    {
        // Convert world position to grid position
        int gridX = Mathf.FloorToInt(position.x / tileSize);
        int gridZ = Mathf.FloorToInt(position.z / tileSize);

        // If we're within bounds, replace the wall with a floor (empty space)
        if (gridX >= 0 && gridX < width && gridZ >= 0 && gridZ < height)
        {
            // Adjust the position based on the tile size for correct placement
            Vector3 floorPosition = new Vector3(gridX * tileSize, 0, gridZ * tileSize);

            // Destroy the wall at this position
            Destroy(grid[gridX, gridZ]);

            // Instantiate a new floor at this position
            grid[gridX, gridZ] = Instantiate(floorPrefab, floorPosition, Quaternion.identity);

            // Mark this tile as visited (now a floor)
            visited[gridX, gridZ] = true;
        }
    }
}
