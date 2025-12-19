using UnityEngine;
using System.Collections.Generic;

public class Dock : MonoBehaviour
{
    [SerializeField] private GameObject dockSpacePrefab;
    private List<DockSpace> spaces = new();

    /// <summary>
    /// Add a certain amount of spaces to the dock
    /// </summary>
    /// <param name="numberOfDockSpaces">Number of spaces.</param>
    public void Initialize(int numberOfDockSpaces)
    {
        for (int i = 0; i < numberOfDockSpaces; i++)
        {
            GameObject dockSpaceGO = Instantiate(dockSpacePrefab, transform);
            spaces.Add(dockSpaceGO.GetComponent<DockSpace>());
        }
        ArrangeDockSpaces();
    }
    
    public DockSpace CheckForFreeSpace()
    {
        foreach (DockSpace dockSpace in spaces)
        {
            if (dockSpace.ShooterBlock == null)
                return dockSpace;
        }
        return null;
    }

    private void ArrangeDockSpaces()
    {
        float dockStartX = 0f;
        float dockEndX = 9f;
        float centerX = (dockStartX + dockEndX) * 0.5f;
        float spacing = 2.25f;

        for (int i = 0; i < spaces.Count; i++)
        {
            float offset = (i - (spaces.Count - 1) / 2f) * spacing;
            float x = centerX + offset;
            spaces[i].transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}