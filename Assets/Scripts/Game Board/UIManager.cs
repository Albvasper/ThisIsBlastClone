using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject gameOverScreen;
    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    public void ShowWinScreen()
    {
        StartCoroutine(DisplayScreenSlowly(winScreen, 2f));
    }

    public void ShowGameOverScreen()
    {
        StartCoroutine(DisplayScreenSlowly(gameOverScreen, 0f));
    }

    public void NextLevelButtonClicked()
    {
        levelManager.GoToNextLevel();
    }

    public void RetryLevelButtonClicked()
    {
        levelManager.RetryLevel();
    }

    private IEnumerator DisplayScreenSlowly(GameObject screen, float delay)
    {
        yield return new WaitForSeconds(delay);
        screen.SetActive(true);
    }
}
