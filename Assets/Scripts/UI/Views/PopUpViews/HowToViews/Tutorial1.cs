using UnityEngine;
using UnityEngine.UI;

public class Tutorial1 : PopUpView
{
    [SerializeField] private Button nextButton;

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(nextButton != null, "Next button not set in how to view 1");
    }

    protected override void Awake()
    {
        base.Awake();

        nextButton.onClick.AddListener(() => UI.PushOverlay(PopUpViewType.Tutorial2));
    }
}
