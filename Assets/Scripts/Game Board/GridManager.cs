using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Singleton that arrages blocks on the grid and checks if blocks are missing to drop down columns.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private const int GridPosZ = 25;
    public Block[,,] grid { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] public int gridX = 10;
    [SerializeField] public int gridY = 12;
    [SerializeField] public int gridZ = 2;

    [Header("Components")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform gridSpawnPoint;

    [Header("Block Materials")]
    public Material Red;
    public Material Yellow;
    public Material Blue;
    public Material Green;
    public Material Orange;
    public Material Pink;
    public Material Cyan;
    public Material Purple;
    
    private LevelManager levelManager;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        levelManager = GetComponent<LevelManager>();
    }
    
    private void OnEnable()
    {
        grid = new Block[gridX, gridY,gridZ];
        InitializeGrid();
    }

    /// <summary>
    /// Deletes a block on the grid waiting one frame
    /// </summary>
    /// <param name="block">Block that will be removed</param>
    public void RemoveBlockDelayed(Block block, float delay)
    {
        StartCoroutine(RemoveBlock(block, delay));
    }

    // Place blocks on the alrady determined grid size
    private void InitializeGrid()
    {
        for (int z = 0; z < gridZ; z++)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int x = 0; x < gridX; x++)
                {
                    // Spawn block on grid
                    Vector3 blockPos = new(x, y, GridPosZ + z);
                    GameObject block = Instantiate (blockPrefab, blockPos, Quaternion.identity, gridSpawnPoint);
                    Block blockComponent = block.GetComponent<Block>();
                    // Add it to the block matrix
                    grid[x,y,z] = blockComponent;
                    // Change block color
                    int index = x + y * gridX + z * gridX * gridY;
                    block.GetComponent<Block>().Initialize(levelManager.level.gridLayout[index]);
                }
            }
        }
    }

    private IEnumerator RemoveBlock(Block block, float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int z = 0; z < gridZ; z++)
                {
                    if (grid[x, y, z] == block)
                    {
                        grid[x, y, z] = null;
                        DropDownColumn(x, z);
                        yield return null;
                    }
                }
            }
        }
    }

    private void DropDownColumn(int x, int z)
    {
        int targetY = 0;

        for (int y = 0; y < gridY; y++)
        {
            Block block = grid[x, y, z];
            if (block == null)
                continue;

            if (y != targetY)
            {
                grid[x, targetY, z] = block;
                grid[x, y, z] = null;
                MoveBlockToNewCell(block, x, targetY, z);
            }
            targetY++;
        }
    }

    // Assign new position to block and start movement
    private void MoveBlockToNewCell(Block block, int x, int y, int z)
    {
        Vector3 newPos = GridToWorld(x, y, z);
        float moveDuration = 0.15f;
        StartCoroutine(LerpBlock(block, newPos, moveDuration));
    }

    // Convert grid position to world position
    private Vector3 GridToWorld(int x, int y, int z)
    {
        return new Vector3(
            gridSpawnPoint.position.x + x,
            gridSpawnPoint.position.y + y,
            gridSpawnPoint.position.z + z
        );
    }

    // Move block slowly to new position
    private IEnumerator LerpBlock(Block block, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = block.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / duration;
            block.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            yield return null;
        }
        block.transform.position = targetPosition;
    }
}
