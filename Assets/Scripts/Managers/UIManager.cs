using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles UI set up and update.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private Image progressBar;

    [Header("SFXs")]
    [SerializeField] private AudioClip winGameSFX;
    [SerializeField] private AudioClip gameOverSFX;

    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
        currentLevelText.text = "Level " + levelManager.level.levelNumber.ToString();
        progressBar.fillAmount = 0f;
    }

    public void UpdateProgressBar(float progress, float maxProgress)
    {
        progressBar.fillAmount = progress / maxProgress;
    }

    public void ShowWinScreen()
    {
        StartCoroutine(DisplayScreenSlowly(winScreen, winGameSFX, 2f));
    }

    public void ShowGameOverScreen()
    {
        StartCoroutine(DisplayScreenSlowly(gameOverScreen, gameOverSFX, 0f));
    }

    public void NextLevelButtonClicked()
    {
        levelManager.GoToNextLevel();
    }

    public void RetryLevelButtonClicked()
    {
        levelManager.RetryLevel();
    }

    private IEnumerator DisplayScreenSlowly(GameObject screen, AudioClip sfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        screen.SetActive(true);
        AudioManager.Instance.PlaySFX(sfx);
    }
}
