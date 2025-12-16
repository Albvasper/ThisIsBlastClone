using UnityEngine;

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
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

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
    }
}
