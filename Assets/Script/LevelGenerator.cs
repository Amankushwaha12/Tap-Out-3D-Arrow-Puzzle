using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject cubePrefab; // Assign your rounded cube prefab here
    public int numberOfCubes = 20; // How many blocks per level
    public float spacing = 1.05f;  // Space between cubes in the grid

    private List<Vector3> occupiedPositions = new List<Vector3>();

    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        occupiedPositions.Clear();
        int cubesSpawned = 0;

        while (cubesSpawned < numberOfCubes)
        {
            // 1. Pick a random grid coordinate near the center
            Vector3 randomPos = new Vector3(
                Mathf.Round(Random.Range(-3, 3)) * spacing,
                0, // Keep it flat on one layer first, add Y for 3D stacks later
                Mathf.Round(Random.Range(-3, 3)) * spacing
            );

            // 2. Make sure the spot is empty
            if (!occupiedPositions.Contains(randomPos))
            {
                // 3. Pick a random escape direction (Up, Down, Left, or Right)
                Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
                Vector3 escapeDir = directions[Random.Range(0, directions.Length)];

                // 4. Instantiate the cube
                GameObject newCube = Instantiate(cubePrefab, randomPos, Quaternion.identity);
                
                // 5. Rotate the cube so its top arrow faces the escape direction
                newCube.transform.rotation = Quaternion.LookRotation(escapeDir);

                // Add to our tracking list
                occupiedPositions.Add(randomPos);
                cubesSpawned++;
            }
        }
    }
}