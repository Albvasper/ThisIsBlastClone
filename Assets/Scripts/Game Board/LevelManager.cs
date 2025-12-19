using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Tracks player progress through a level and checks win/lose condition
/// </summary>
public class LevelManager : MonoBehaviour
{
    public Level level;
    [SerializeField] private Image progressBar;
    private float progress = 0f;
    private float maxProgress;
    private UIManager uiManager;

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
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

