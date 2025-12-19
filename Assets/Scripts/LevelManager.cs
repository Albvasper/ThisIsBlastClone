using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tracks player progress through a level and checks win/lose condition
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    private float progress = 0f;
    private float maxProgress;

    private void Awake()
    {
        progressBar.fillAmount = progress;
    }

    private void Start()
    {
        maxProgress = GridManager.Instance.grid.Length;
    }

    /// <summary>
    /// Adds 1 to the progress of the level. It gets called when the player destroys a block.
    /// </summary>
    public void MakeProgress()
    {
        progress++;
        progressBar.fillAmount = progress / maxProgress;
    }
}

