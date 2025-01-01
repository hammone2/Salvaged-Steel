using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomLevelGenerator : MonoBehaviour
{
    // Grid parameters
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float tileSize = 1f;

    // Prefabs for level generation
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject pickupPrefab;

    // Cellular Automata parameters
    [Range(0f, 1f)] public float fillProbability = 0.25f;  // Initial probability of walls
    public int birthLimit = 4; // Wall-to-floor rule: number of neighboring walls required for floor to become wall
    public int deathLimit = 3; // Floor-to-wall rule: number of neighboring walls required for wall to become floor
    public int iterations = 5; // Number of iterations to run the CA algorithm

    private int[,] map; // 2D array to store the map data (1 for wall, 0 for floor)

    private void Start()
    {
        GenerateLevel();
        //BakeNavMesh();  // Bake NavMesh after generating the level
    }

    public void GenerateLevel()
    {
        // Initialize the map with random walls and floors
        map = new int[gridWidth, gridHeight];
        InitializeMap();

        // Run the cellular automata algorithm
        for (int i = 0; i < iterations; i++)
        {
            map = SimulateCA(map);
        }

        // Generate the level objects based on the final map
        BuildLevel();
    }

    // Initialize map with random walls/floors
    private void InitializeMap()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                if (Random.value < fillProbability)
                    map[x, z] = 1; // Wall
                else
                    map[x, z] = 0; // Floor
            }
        }
    }

    // Cellular Automata simulation step
    private int[,] SimulateCA(int[,] map)
    {
        int[,] newMap = new int[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                int neighborWalls = CountNeighborWalls(x, z);

                // Apply rules: Birth (floor to wall) and Death (wall to floor)
                if (map[x, z] == 1)
                {
                    newMap[x, z] = (neighborWalls >= deathLimit) ? 1 : 0; // Wall stays if there are enough neighboring walls
                }
                else
                {
                    newMap[x, z] = (neighborWalls > birthLimit) ? 1 : 0; // Floor becomes wall if there are too many neighboring walls
                }
            }
        }

        return newMap;
    }

    // Count neighboring walls around a tile (8-connected neighbors)
    private int CountNeighborWalls(int x, int z)
    {
        int wallCount = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                // Skip the center tile (itself)
                if (dx == 0 && dz == 0) continue;

                int nx = x + dx;
                int nz = z + dz;

                // Wrap-around or wall bounds
                if (nx < 0 || nz < 0 || nx >= gridWidth || nz >= gridHeight)
                    wallCount++; // Treat out-of-bound cells as walls
                else
                    wallCount += map[nx, nz]; // Count neighboring walls
            }
        }
        return wallCount;
    }

    // Build the level from the final map
    private void BuildLevel()
    {
        // Destroy any existing objects in the scene
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate the floor and wall objects based on the final map
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);

                if (map[x, z] == 1) // Wall
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                else // Floor
                {
                    //GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity, transform);

                    // Add a NavMeshSurface component to the floor tile to make it walkable
                    /*NavMeshSurface navMeshSurface = floor.GetComponent<NavMeshSurface>();
                    if (navMeshSurface == null)
                    {
                        navMeshSurface = floor.AddComponent<NavMeshSurface>();
                    }

                    navMeshSurface.collectObjects = CollectObjects.Children;  // Collect objects under this surface 

                    // Randomly place pickups
                    if (Random.value < 0.05f)  // Change the chance as needed
                    {
                        Instantiate(pickupPrefab, position + new Vector3(0, 1, 0), Quaternion.identity, transform); // Offset to make pickup above the floor
                    } */
                }
            }
        }
    }

    // Bake the NavMesh after the level is generated
    /*public void BakeNavMesh()
    {
        // Find all the NavMeshSurface components in the scene and bake them
        NavMeshSurface[] surfaces = FindObjectsOfType<NavMeshSurface>();
        foreach (NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();  // Bake the NavMesh for each surface
        }
    }*/
}
