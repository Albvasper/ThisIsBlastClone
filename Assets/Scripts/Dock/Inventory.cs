using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Dock dock;
    [SerializeField] private List<ShooterBlock> availableShooterBlocks;

    public void TryDeployShooterBlock(ShooterBlock shooterBlock)
    {   
        DockSpace freeDockSpace = dock.CheckForFreeSpace();
        // If there free spaces on the dock, deploy the shooter block
        if (freeDockSpace != null && !shooterBlock.OnDock)
        {
            freeDockSpace.ShooterBlock = shooterBlock;
            shooterBlock.transform.position = freeDockSpace.transform.position;
            // Try to activate shooter
            shooterBlock.OnDock = true;
            shooterBlock.ReadyToShoot();
        }
    }
}