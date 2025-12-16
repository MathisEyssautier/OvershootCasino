using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject tutorialPanel;

    private bool tutorialActive = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tutorialPanel.SetActive(false);
        tutorialActive = true;
    }

    // Appelé quand la caméra a fini son mouvement
    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        tutorialActive = true;
    }

    // Bouton PLAY
    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
        tutorialActive = false;
    }

    public bool CanPlay()
    {
        return !tutorialActive;
    }
}
