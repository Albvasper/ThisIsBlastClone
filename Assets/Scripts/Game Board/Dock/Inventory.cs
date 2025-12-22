using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages available shooter blocks and deploys them.
/// </summary>
public class Inventory : MonoBehaviour
{
    public ShooterBlock[,] grid { get; private set; }
    [SerializeField] private float blockSpacing = 1.2f;
    [SerializeField] private Dock dock;
    [SerializeField] private AudioClip deployShooterSFX;

    private int gridX;
    private int gridY;
    private List<ShooterBlock> availableShooterBlocks = new();

    private void Awake()
    {
        // Get all available shooter blocks
        foreach (Transform child in transform)
        {
            availableShooterBlocks.Add(child.GetComponent<ShooterBlock>());
        }
    }

    /// <summary>
    /// Sets the size of the inventory grid.
    /// </summary>
    /// <param name="numberOfCols">Number of columns of the inventory grid.</param>
    public void Initialize(int numberOfCols)
    {
        gridX = numberOfCols;
        gridY = Mathf.CeilToInt((float)availableShooterBlocks.Count / gridX);
        grid = new ShooterBlock[gridX, gridY];
        FillArray();
        ArrangeGrid();
        StackShootersAmmo();
    }

    /// <summary>
    /// Checks for free spaces on dock. If there are, removes it from inventory and deploys it on dock.
    /// </summary>
    /// <param name="shooterBlock">Shooter block that will be deployed.</param>
    public void TryDeployShooterBlock(ShooterBlock shooterBlock)
    {   
        if (TryDeployConnectedShooter(shooterBlock))
            return;

        TryDeploySingleShooter(shooterBlock);
    }

    // Get shooter blocks from the list and put them into the array
    private void FillArray()
    {
        int index = 0;

        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (index >= availableShooterBlocks.Count)
                    return;
                // Get shooter block from list and put it into the 2D array
                grid[x, y] = availableShooterBlocks[index];
                index++;
            }
        }
    }

    // Arrange shooter blocks from the 2D array to level 
    private void ArrangeGrid()
    {
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                ShooterBlock block = grid[x, y];
                if (block == null)
                    continue;
                MoveShooterToNewCell(block, x, y);
            }
        }
        MakeTopLayerClickable();
    }

    // If the shooter is a connected shooter try to deploy it. If it is a connected shooter return true.
    private bool TryDeployConnectedShooter(ShooterBlock shooterBlock)
    {
        ConnectedShooterBlock connectedShooter = shooterBlock.GetComponent<ConnectedShooterBlock>();
        if (connectedShooter == null)
            return false;

        if (!CanDeployConnectedShooter(connectedShooter))
            return true; 

        var (dockSpace1, dockSpace2) = dock.CheckForFreeSpaces();
        if (dockSpace1 == null || dockSpace2 == null)
            return true;

        DeployConnectedShooter(connectedShooter, dockSpace1, dockSpace2);
        return true;
    }

    // Check if the connected shooter can be deployed following certain rules.
    private bool CanDeployConnectedShooter(ConnectedShooterBlock connectedShooter)
    {
        if (connectedShooter.OnDock)
        {
            return false;
        }
        if (connectedShooter.OtherShooterBlock == null)
        {
            return false;
        }
        if (!connectedShooter.OtherShooterBlock.readyToDeploy)
        {
            return false;
        }
        return true;
    }

    // Deploy connected shooters on dock. First remove them form the inventory and then assign them to a dock space.
    private void DeployConnectedShooter(ConnectedShooterBlock connected, DockSpace dockSpace1, DockSpace dockSpace2)
    {   
        AudioManager.Instance.PlaySFX(deployShooterSFX);
        RemoveFromShooterFromInventory(connected);
        RemoveFromShooterFromInventory(connected.OtherShooterBlock);
        dockSpace1.AssignShooterBlock(connected);
        dockSpace2.AssignShooterBlock(connected.OtherShooterBlock);
        AudioManager.Instance.PlaySFX(deployShooterSFX);
    }   

    // If the shooter is a single shooter try to deploy it
    private void TryDeploySingleShooter(ShooterBlock shooterBlock)
    {
        if (shooterBlock.OnDock)
            return;

        DockSpace dockSpace = dock.CheckForFreeSpace();
        if (dockSpace == null)
            return;
        
        AudioManager.Instance.PlaySFX(deployShooterSFX);
        dockSpace.AssignShooterBlock(shooterBlock);
        RemoveFromShooterFromInventory(shooterBlock);
        dock.CheckForShooterMerges();
    }

    // Fill shooters with ammo by counting the blocks on the grid.
    private void StackShootersAmmo()
    {
        Dictionary<BlockColor, int> colorsOnGrid = CountBlocksByColor();
        Dictionary<BlockColor, List<ShooterBlock>> shootersByColor = GroupShootersByColor();

        // Distribute ammo evenly among shooters of the same color
        DistributeAmmo(colorsOnGrid, shootersByColor);
    }

    private Dictionary<BlockColor, int> CountBlocksByColor()
    {
        Dictionary<BlockColor, int> colorsOnGrid = new();

        foreach (Block block in GridManager.Instance.grid)
        {
            if (block == null) continue;

            if (!colorsOnGrid.ContainsKey(block.Color))
                colorsOnGrid[block.Color] = 0;

            colorsOnGrid[block.Color]++;
        }
        return colorsOnGrid;
    }

    private Dictionary<BlockColor, List<ShooterBlock>> GroupShootersByColor()
    {
        Dictionary<BlockColor, List<ShooterBlock>> shootersByColor = new();

        foreach (ShooterBlock shooter in availableShooterBlocks)
        {
            if (shooter == null) continue;

            if (!shootersByColor.ContainsKey(shooter.Color))
                shootersByColor[shooter.Color] = new List<ShooterBlock>();

            shootersByColor[shooter.Color].Add(shooter);
        }
        return shootersByColor;
    }
    
    private void DistributeAmmo(
        Dictionary<BlockColor, int> colorsOnGrid, 
        Dictionary<BlockColor, List<ShooterBlock>> shootersByColor)
    {
        foreach (var kvp in shootersByColor)
        {
            BlockColor color = kvp.Key;
            List<ShooterBlock> shooters = kvp.Value;

            if (!colorsOnGrid.ContainsKey(color)) 
                continue;

            int totalAmmo = colorsOnGrid[color];
            int ammoPerShooter = totalAmmo / shooters.Count;
            int remainder = totalAmmo % shooters.Count;

            for (int i = 0; i < shooters.Count; i++)
            {
                int ammoToGive = ammoPerShooter + (i < remainder ? 1 : 0);
                shooters[i].AssignAmmo(ammoToGive);
            }
        }
    }

    // Remove shooter block from inventory
    private void RemoveFromShooterFromInventory(ShooterBlock block)
    {
        availableShooterBlocks.Remove(block);

        for (int x = 0; x < gridX; x++)
        {
            for (int y = gridY - 1; y >= 0; y--)
            {
                if (grid[x, y] == block)
                {
                    MoveColumnUp(x, y);
                    MakeTopLayerClickable();
                    return;
                }
            }
        }
    }

    // Since a block was removed, move the whole column one cell up.
    private void MoveColumnUp(int column, int removedRow)
    {
        for (int y = removedRow; y < gridY - 1; y++)
        {
            grid[column, y] = grid[column, y + 1];
            if (grid[column, y] != null)
            {
                MoveShooterToNewCell(grid[column, y], column, y);
            }
        }
        grid[column, gridY - 1] = null;
        //ArrangeGrid();
    }
    
    // Assign new position to shooter block
    private void MoveShooterToNewCell(ShooterBlock shooter, int x, int y)
    {

        float totalWidth = (gridX - 1) * blockSpacing;
        float startX = transform.position.x - totalWidth / 2f;
        float startY = transform.position.y;
        float moveDuration = 0.15f;

        Vector3 worldPos = new (
            startX + x * blockSpacing,
            startY - y * blockSpacing,
            transform.position.z
        );
        StartCoroutine(LerpBlock(shooter, worldPos, moveDuration));
    }

    // Move block slowly to new position
    private IEnumerator LerpBlock(ShooterBlock block, Vector3 targetPosition, float duration)
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
    
    private void MakeTopLayerClickable()
    {
        int clickableLayer = LayerMask.NameToLayer("ClickableShooter");
        for (int x = 0; x < gridX; x++)
        {
            ShooterBlock shooter = grid[x, 0];
            if (shooter == null) 
                continue;
            shooter.gameObject.layer = clickableLayer;
            if (shooter.supriseShooter)
                shooter.AssignMaterial(shooter.Color);
            shooter.readyToDeploy = true;
        }
    }
}