using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : PopUpView
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(nextButton != null, "Next button not set in how to view 2");
        Debug.Assert(backButton != null, "Back button not set in how to view 2");
    }

    protected override void Awake()
    {
        base.Awake();

        nextButton.onClick.AddListener(() => Controller.PushOverlay(PopUpViewType.Tutorial3, new NoData()));
        backButton.onClick.AddListener(() => Controller.PopOverlay());
    }
}
