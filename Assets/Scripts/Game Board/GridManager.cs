using UnityEngine;
using System.Collections;

/// <summary>
/// Singleton that arrages blocks on the grid and checks if blocks are missing to drop down columns.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private const int GridPosZ = 25;

    [Header("Grid Settings")]
    [SerializeField] public int gridX = 10;
    [SerializeField] public int gridY = 12;

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

    public Block[,] grid { get; private set; }
    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }
    private void OnEnable()
    {
        grid = new Block[gridX, gridY];
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        InitializeGrid();
    }

    private void Update()
    {
        CheckForMissingBlock();
    }

    // Place blocks on the alrady determined grid size
    private void InitializeGrid()
    {
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                // Spawn block on grid
                Vector2 blockPos = new(x, y);
                GameObject block = Instantiate
                (
                    blockPrefab, 
                    new Vector3(blockPos.x, blockPos.y, GridPosZ), 
                    Quaternion.identity
                );
                Block blockComponent = block.GetComponent<Block>();
                // Add it to the block matrix
                grid[x,y] = blockComponent;
                block.transform.SetParent(gridSpawnPoint);
                // Change block color
                block.GetComponent<Block>().Initialize(levelManager.level.gridLayout[x + y * gridX]);
            }
        }
    }
    
    private void DropDownColumn(int x)
    {
        int targetRow = 0;
        for (int currentRow = 0; currentRow < gridY; currentRow++)
        {
            Block block = grid[x, currentRow];
            if (block == null)
            continue;

            if (currentRow != targetRow)
            {
                // Update 2D array
                grid[x, targetRow] = block;
                grid[x, currentRow] = null;
                // Update game world
                MoveBlockToNewCell(block, x, targetRow);
            }
            targetRow++;
        }
    }

    // Assign new position to block and start movement
    private void MoveBlockToNewCell(Block block, int x, int y)
    {
        Vector3 newPos = new(x, y, GridPosZ);
        float moveDuration = 0.15f;
        StartCoroutine(LerpBlock(block, newPos, moveDuration));
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

    // Iterate through bottom row and check for missing blocks
    private void CheckForMissingBlock()
    {
        // TODO: THIS SHOULD ONLY BE CALLED WHEN DESTROYING A BLOCK?
        for (int i = 0; i < gridX; i++)
        {
            if (grid[i, 0] == null)
            {   
                DropDownColumn(i);
            }
        }
    }
}
