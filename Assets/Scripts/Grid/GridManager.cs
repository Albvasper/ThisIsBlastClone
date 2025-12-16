using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private const int GridPosZ = 25;

    [Header("Grid Settings")]
    [SerializeField] private int gridX = 10;
    [SerializeField] private int gridY = 12;

    [Header("Components")]
    [SerializeField] private GameObject blockPrefab;

    [Header("Level")]
    [SerializeField] private Level level;

    [Header("Block Materials")]
    public Material Red;
    public Material Yellow;
    public Material Blue;
    public Material Green;
    public Material Orange;
    public Material Pink;
    public Material Cyan;
    public Material Purple;

    private Block[,] grid;

    private void Awake()
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
    }

    private void Start()
    {
        InitializeGrid();
    }

    private void Update()
    {
        for (int i = 0; i < gridX; i++)
        {
            // Check if any of the bottom row blocks is missing
            if (grid[i, 0] == null)
            {   
                // if a bottom block is missing drop the whole column one
                DropDownColumn(i);
            }
        }
    }

    private void InitializeGrid()
    {
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                // Spawn block on grid
                Vector2 blockPos = new(x, y);
                GameObject block = Instantiate(blockPrefab, new Vector3(blockPos.x, blockPos.y, GridPosZ), Quaternion.identity);
                grid[x,y] = block.GetComponent<Block>();
                block.transform.SetParent(transform);
                // Change block color
                block.GetComponent<Block>().Initialize(level.gridLayout[x + y * gridX]);
            }
        }
    }

    private void DropDownColumn(int x)
    {

        for (int currentRow = 0; currentRow < gridY; currentRow++)
        {
            Block block = grid[x, currentRow];
            if (block != null)
            {
                // Update 2D array
                grid[x, currentRow - 1] = block;
                grid[x, currentRow] = null;
                // Update game world
                MoveBlockToNewCell(block, x, currentRow - 1);
            }
        }
    }

    private void MoveBlockToNewCell(Block block, int x, int y)
    {
        Vector3 newPos = new(x, y, GridPosZ);
        // TODO: Apply lerp to block movement
        block.transform.position = newPos;
    }
}
