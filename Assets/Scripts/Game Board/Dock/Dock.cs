using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dock : MonoBehaviour
{
    [SerializeField] private GameObject dockSpacePrefab;
    public List<DockSpace> Spaces {get; private set; } = new();

    /// <summary>
    /// Add a certain amount of spaces to the dock
    /// </summary>
    /// <param name="numberOfDockSpaces">Number of spaces.</param>c
    public void Initialize(int numberOfDockSpaces)
    {
        for (int i = 0; i < numberOfDockSpaces; i++)
        {
            GameObject dockSpaceGO = Instantiate(dockSpacePrefab, transform);
            DockSpace dockSpace = dockSpaceGO.GetComponent<DockSpace>();
            dockSpace.Initialize(i);
            Spaces.Add(dockSpace);
        }
        ArrangeDockSpaces();
    }
    
    /// <summary>
    /// Return a free space on dock if available.
    /// </summary>
    /// <returns></returns>
    public DockSpace CheckForFreeSpace()
    {
        foreach (DockSpace dockSpace in Spaces)
        {
            if (dockSpace.ShooterBlock == null)
                return dockSpace;
        }
        return null;
    }

    /// <summary>
    /// Return a 2 free spaces on dock for connected shooters.
    /// </summary>
    /// <returns></returns>
    public (DockSpace space1, DockSpace space2) CheckForFreeSpaces()
    {
        DockSpace first = null;
        DockSpace second = null;

        foreach (DockSpace dockSpace in Spaces)
        {
            if (dockSpace.ShooterBlock != null)
                continue;
            
            if (first == null)
            {
                first = dockSpace;
            }
            else
            {
                second = dockSpace;
                return (first, second);
            }
        }

        return (null, null);
    }

    /// <summary>
    // Iterate through available shooters and compares their color. If 3 are the same color: merge them.
    /// </summary>
    /// <returns></returns>
    public void CheckForShooterMerges()
    {
        int count = 0;
        BlockColor currentColor = BlockColor.None;
        List<int> indices = new();

        for (int i = 0; i < Spaces.Count; i++)
        {
            ShooterBlock shooter = Spaces[i].ShooterBlock;
            if (shooter == null)
            {
                Reset();
                continue;
            }
            if (shooter.Color != currentColor)
            {
                Reset();
                currentColor = shooter.Color;
            }
            count++;
            indices.Add(i);
            if (count == 3)
            {
                MergeShooters(indices);
                return;
            }
        }

        void Reset()
        {
            count = 0;
            currentColor = BlockColor.None;
            indices.Clear();
        }
    }
    
    /// <summary>
    /// Return if dock spaces are full or not
    /// </summary>
    /// <returns></returns>
    public bool IsFull()
    {
        foreach (DockSpace space in Spaces)
        {
            if (space.ShooterBlock == null)
                return false;
        }
        return true;
    }

    public bool EveryShooterStoppedFiring()
    {
        foreach (DockSpace space in Spaces)
        {
            if (!space.ShooterBlock.HasStoppedShooting())
                return false;
        }
        return true;
    }

    // Visually space out dock spaces
    private void ArrangeDockSpaces()
    {
        float dockStartX = 0f;
        float dockEndX = 9f;
        float centerX = (dockStartX + dockEndX) * 0.5f;
        float spacing = 2.25f;

        for (int i = 0; i < Spaces.Count; i++)
        {
            float offset = (i - (Spaces.Count - 1) / 2f) * spacing;
            float x = centerX + offset;
            Spaces[i].transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }

    private void MergeShooters(List<int> indices)
    {
        int extraAmmo = 0;
        // Mid index of the 3 shooters
        int centerIndex = indices[1];
        // Remove side shooters
        foreach (int i in indices)
        {
            Spaces[i].ShooterBlock.StopShooting();
            if (i == centerIndex) 
                continue;
            extraAmmo += Spaces[i].ShooterBlock.Ammo;
            StartCoroutine(MergeShooterAnimation(i, Spaces[centerIndex].ShooterBlock));
        }
        // Upgrade mmiddle shooter block
        ShooterBlock mergedShooter = Spaces[centerIndex].ShooterBlock;
        mergedShooter.Upgrade(extraAmmo);
    }

    private IEnumerator MergeShooterAnimation(int mergingBlockIndex, ShooterBlock shooterBlock)
    {
        const float duration = 0.5f;
        float counter = 0;
        Vector3 startingPosition = Spaces[mergingBlockIndex].ShooterBlock.transform.localPosition;

        while(counter < duration)
        {
            counter += Time.deltaTime;
            float t = counter / duration;
            Spaces[mergingBlockIndex].ShooterBlock.transform.localPosition =
                Vector3.Lerp(startingPosition, shooterBlock.transform.localPosition, t);
            yield return null;
        }
        // Destroy shooter block once merged
        Destroy(Spaces[mergingBlockIndex].ShooterBlock.gameObject);
        shooterBlock.ReadyToShoot();
    }
}