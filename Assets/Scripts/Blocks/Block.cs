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
    Purple,
    None
}

public class Block : MonoBehaviour
{
    [Header("Properties")]
    public BlockColor Color;
    protected MeshRenderer meshRenderer;
    
    protected virtual void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Assign a color and material to the block.
    /// </summary>
    /// <param name="color">Color that will be assigned.</param>
    public void AssignMaterial(BlockColor color)
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
            case BlockColor.None: Debug.LogWarning("No material assigned!"); break;
        }
        Color = color;
    }
    
    /// <summary>
    /// Handles destruction of the block
    /// </summary>
    public void Die()
    {
        StartCoroutine(DeathAnimation());
    }

    // Make block big at first then scale it down overtime while rotating it
    protected virtual IEnumerator DeathAnimation()
    {
        const float duration = 0.35f;
        float counter = 0;
        float impactOffset = 0.1f;
        float impactScale = 1.5f;
        float impactRotation = 10f;

        Vector3 startingPosition = transform.localPosition;
        Vector3 finalPosition = new(
            transform.localPosition.x + Random.Range(-impactOffset, impactOffset), 
            transform.localPosition.y + Random.Range(-impactOffset, impactOffset),
            transform.localPosition.z );
        Vector3 startingScale = new(impactScale, impactScale, impactScale);
        Vector3 finalScale = Vector3.zero;
        Quaternion startingRotation = transform.rotation;
        Quaternion finalRotation = transform.rotation * 
            Quaternion.Euler( 0, 0, Random.Range(-impactRotation, impactRotation));
        
        while(counter < duration)
        {
            counter += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startingPosition, finalPosition, counter/duration);
            transform.localScale = Vector3.Lerp(startingScale, finalScale, counter/duration);
            transform.rotation = Quaternion.Slerp(startingRotation, finalRotation, counter/duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
