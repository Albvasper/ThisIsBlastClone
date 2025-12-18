using UnityEngine;
using System.Collections.Generic;

public class Dock : MonoBehaviour
{
    [SerializeField] private List<DockSpace> spaces;

    public DockSpace CheckForFreeSpace()
    {
        foreach (DockSpace dockSpace in spaces)
        {
            if (dockSpace.ShooterBlock == null)
                return dockSpace;
        }
        return null;
    }
}