using UnityEngine;
using System.Collections;

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
    [SerializeField] private GameObject piggyBankPrefab;
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
    public Material Surprise;
    
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
    
    /// <summary>
    /// Read the level layout and place block accordingly. 
    /// </summary>
    /// <param name="level">Level scriptable object</param>
    public void InitializeGrid(Level level)
    {
        grid = new Block[gridX, gridY,gridZ];

        for (int z = 0; z < gridZ; z++)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int x = 0; x < gridX; x++)
                {
                    int index = x + y * gridX + z * gridX * gridY;
                    Vector3 blockPos = new(x, y, GridPosZ + z);
                    Block block;
                    // TODO: Extract methods!
                    // Piggy bank
                    if (level.gridLayout[index] == BlockColor.PiggyBank)
                    {
                        GameObject piggyBankGO = 
                            Instantiate (piggyBankPrefab, blockPos, Quaternion.identity, gridSpawnPoint);
                        block = piggyBankGO.GetComponent<PiggyBank>();
                    } else
                    {
                        GameObject blockGO = 
                            Instantiate (blockPrefab, blockPos, Quaternion.identity, gridSpawnPoint);
                        block = blockGO.GetComponent<Block>();
                        // Change block color
                        block.AssignMaterial(level.gridLayout[index]);
                    }
                    // Add it to the block matrix
                    grid[x,y,z] = block;
                }
            }
        }
    }

    /// <summary>
    /// Deletes a block on the grid waiting one frame
    /// </summary>
    /// <param name="block">Block that will be removed</param>
    public void RemoveBlock(Block block)
    {
        levelManager.MakeProgress();

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int z = 0; z < gridZ; z++)
                {
                    if (grid[x, y, z] == block)
                    {
                        grid[x, y, z] = null;
                        StartCoroutine(DropDownColumn(x, z));
                    }
                }
            }
        }
    }

    private IEnumerator DropDownColumn(int x, int z)
    {
        const float Delay = 0.35f;
        int targetY = 0;

        yield return new WaitForSeconds(Delay);
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
