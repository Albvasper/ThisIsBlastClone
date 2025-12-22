using UnityEngine;
using System.Collections;

/// <summary>
/// Shooter block that is connected to another shooter block.
/// </summary>
public class ConnectedShooterBlock : ShooterBlock
{
    [SerializeField] public ConnectedShooterBlock OtherShooterBlock;
    
    private LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        if (OtherShooterBlock != null)
        {
            lineRenderer.SetPosition(1, OtherShooterBlock.transform.position);
            DrawConnection(OtherShooterBlock);
        }
    }

    // Moves the block outside of the camera view and then destroys it.
    protected override IEnumerator DeathAnimation(int direction)
    {
        lineRenderer.enabled = false;
        StartCoroutine(base.DeathAnimation(direction));
        yield return null;
    }

    // Draw connection between connected blocks
    private void DrawConnection(ConnectedShooterBlock other)
    {
        // Calculate mid point between blocks
        Vector3 midPoint = (transform.position + other.transform.position) * 0.5f;
        DrawConnectionLine(midPoint);
    }

    // Draw the line and apply respective material
    private void DrawConnectionLine(Vector3 endPoint)
    {
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.material = GridManager.Instance.GetMaterial(Color);
    }
}
