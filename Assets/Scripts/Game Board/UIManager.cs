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
        StartCoroutine(DisplayScreenSlowly(winScreen));
    }

    public void ShowGameOverScreen()
    {
        StartCoroutine(DisplayScreenSlowly(gameOverScreen));
    }

    public void NextLevelButtonClicked()
    {
        levelManager.GoToNextLevel();
    }

    public void RetryLevelButtonClicked()
    {
        levelManager.RetryLevel();
    }

    private IEnumerator DisplayScreenSlowly(GameObject screen)
    {
        float delay = 2f;
        yield return new WaitForSeconds(delay);
        screen.SetActive(true);
    }
}
