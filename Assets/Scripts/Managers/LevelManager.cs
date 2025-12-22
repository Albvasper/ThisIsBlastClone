using UnityEngine;
using UnityEngine.SceneManagement;

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

    private GridManager gridManager;
    private UIManager uiManager;
    private float progress = 0f;
    private float maxProgress;
    private bool gameIsOver = false;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        uiManager = GetComponent<UIManager>();
    }

    private void Start()
    {
        gridManager.InitializeGrid(level);
        inventory.Initialize(numberOfInventoryCols);
        dock.Initialize(numberOfDockSlots);
        maxProgress = GridManager.Instance.grid.Length;
    }

    private void Update()
    {
        //CheckGameOverCondition();
    }

    /// <summary>
    /// Adds 1 to the progress of the level. It gets called when the player destroys a block.
    /// </summary>
    public void MakeProgress()
    {
        progress++;
        uiManager.UpdateProgressBar(progress, maxProgress);
        CheckWinCondition();
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RetryLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void CheckGameOverCondition()
    {
        if (gameIsOver)
            return;

        if (dock.IsFull() && dock.EveryShooterStoppedFiring())
        {   
            gameIsOver = true;
            uiManager.ShowGameOverScreen();
        }
    }
    
    private void CheckWinCondition()
    {
        if (progress >= maxProgress)
        {
            uiManager.ShowWinScreen();
        }
    }
}

