using UnityEngine;
using UnityEngine.UI;

public class Tutorial1 : TutorialView
{
    [SerializeField] private Button nextButton;

    void OnValidate()
    {
        Debug.Assert(nextButton != null, "Next button not set in how to view 1");
    }

    void Awake()
    {
        nextButton.onClick.AddListener(() => Manager.SwitchTutorial(TutorialViewType.Tutorial2));
    }
}
