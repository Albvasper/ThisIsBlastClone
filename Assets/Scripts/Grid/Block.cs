using UnityEngine;
using System.Collections;

public enum BlockColor
{
    Red,
    Yellow,
    Blue,
    Green,
    Orange,
    Pink,
    Cyan,
    Purple
}

public class Block : MonoBehaviour
{
    public BlockColor Color;
    private MeshRenderer meshRenderer;
    
    protected virtual void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Assign a color and material to the block.
    /// </summary>
    /// <param name="color">Color that will be assigned.</param>
    public void Initialize(BlockColor color)
    {
        switch (color)
        {
            case BlockColor.Red: meshRenderer.material = GridManager.Instance.Red; break;
            case BlockColor.Yellow: meshRenderer.material = GridManager.Instance.Yellow; break;
            case BlockColor.Blue: meshRenderer.material = GridManager.Instance.Blue; break;
            case BlockColor.Green: meshRenderer.material = GridManager.Instance.Green; break;
            case BlockColor.Orange: meshRenderer.material = GridManager.Instance.Orange; break;
            case BlockColor.Pink: meshRenderer.material = GridManager.Instance.Pink; break;
            case BlockColor.Cyan: meshRenderer.material = GridManager.Instance.Cyan; break;
            case BlockColor.Purple: meshRenderer.material = GridManager.Instance.Purple; break;
        }
        Color = color;
    }
    
    /// <summary>
    /// Handles destruction of the block
    /// </summary>
    public void Kill()
    {
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation()
    {
        const float duration = 0.1f;
        float counter = 0;
        Vector3 startingScale = new(1.5f, 1.5f, 1.5f);
        Vector3 finalScale = Vector3.zero;
        Quaternion startingRotation = transform.rotation;
        Quaternion finalRotation = transform.rotation * Quaternion.Euler(0, 0, 15f);
        
        while(counter < duration)
        {
            counter += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startingScale, finalScale, counter/duration);
            transform.rotation = Quaternion.Slerp(startingRotation, finalRotation, counter/duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
