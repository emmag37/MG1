using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : TutorialView
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;

    void OnValidate()
    {
        Debug.Assert(nextButton != null, "Next button not set in how to view 2");
        Debug.Assert(backButton != null, "Back button not set in how to view 2");
    }

    void Awake()
    {
        nextButton.onClick.AddListener(() => Manager.SwitchTutorial(TutorialViewType.Tutorial3));
        backButton.onClick.AddListener(() => Manager.SwitchTutorial(TutorialViewType.Tutorial1));
    }
}
