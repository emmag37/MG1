using UnityEngine;
using UnityEngine.UI;

public class Tutorial3 : TutorialView
{
    [SerializeField] private Button backButton;

    void OnValidate()
    {
        Debug.Assert(backButton != null, "Back button not set in how to view 3");
    }

    void Awake()
    {
        backButton.onClick.AddListener(() => Manager.SwitchTutorial(TutorialViewType.Tutorial2));
    }
}
