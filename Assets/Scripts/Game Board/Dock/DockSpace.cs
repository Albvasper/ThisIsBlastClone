using UnityEngine;

public class DockSpace : MonoBehaviour
{
    public ShooterBlock ShooterBlock;
    private Dock dock;
    private int index;

    private void Awake()
    {
        dock = GetComponentInParent<Dock>();
    }

    private void Update()
    {
        CheckShooterBlockAmmo();
    }

    /// <summary>
    /// Initializes dock space by giving it the index on the dock
    /// </summary>
    /// <param name="index">Index on dock</param>
    public void Initialize(int index)
    {
        this.index = index;
    }

    public void AssignShooterBlock(ShooterBlock shooterBlock)
    {
        // Move shooter to dock space
        ShooterBlock = shooterBlock;
        shooterBlock.transform.position = transform.position;
        shooterBlock.OnDock = true;
        shooterBlock.ReadyToShoot();
    }


    public void RemoveShooterBlock() {
        ShooterBlock.Die(CalculateShooterRemovalDirection());
        ShooterBlock = null;
    }

    private void CheckShooterBlockAmmo()
    {
        if (ShooterBlock == null)
            return;

        // If shooter is out of ammo remove it from dock
        if (ShooterBlock.Ammo <= 0)
        {
            RemoveShooterBlock();
        }
    }
    
    private int CalculateShooterRemovalDirection()
    {
        int dockSpaces = dock.Spaces.Count - 1;
        if (index < dockSpaces)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}