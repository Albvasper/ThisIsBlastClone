using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public bool tutorialRequired;

    [SerializeField] private GameObject tutorialScreen;
    [Range(1, 3)][SerializeField] private int numberOfBlocksNeeded;

    private int blockCounter = 0;

    private void Awake()
    {
        Instance = this;
        if (tutorialRequired)
            tutorialScreen.SetActive(true);
        else 
            tutorialScreen.SetActive(false);
    }

    public void ProgressTutorial()
    {
        blockCounter++;
        if (blockCounter >= numberOfBlocksNeeded)
        {
            tutorialScreen.SetActive(false);
        }
    }
}
