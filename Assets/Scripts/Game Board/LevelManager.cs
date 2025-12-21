using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// TODO: Remove win and lose condition form here!
/// <summary>
/// Sets up level and tracks player progress through a level.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Level Properties")]
    public Level level;
    [Range(1, 5)] [SerializeField] private int numberOfDockSlots;
    [Range(1, 5)] [SerializeField] private int numberOfInventoryCols;

    [Header("Components")]
    [SerializeField] private Dock dock;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GridManager gridManager;
    // TODO: MOVE THIS TO UI MANAGER!
    [SerializeField] private Image progressBar;

    private UIManager uiManager;
    private float progress = 0f;
    private float maxProgress;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        uiManager = GetComponent<UIManager>();
        progressBar.fillAmount = progress;

    }

    private void Start()
    {
        gridManager.InitializeGrid(level);
        inventory.Initialize(numberOfInventoryCols);
        dock.Initialize(numberOfDockSlots);
        maxProgress = GridManager.Instance.grid.Length;
    }

    /// <summary>
    /// Adds 1 to the progress of the level. It gets called when the player destroys a block.
    /// </summary>
    public void MakeProgress()
    {
        progress++;
        progressBar.fillAmount = progress / maxProgress;
        CheckWinCondition();
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RetryLevel()
    {
    }

    public void CheckGameOverCondition()
    {
    }
    
    private void CheckWinCondition()
    {
        if (progress >= maxProgress)
        {
            uiManager.ShowWinScreen();
        }
    }
}

