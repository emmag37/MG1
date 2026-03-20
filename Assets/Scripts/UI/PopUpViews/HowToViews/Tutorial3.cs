using UnityEngine;
using UnityEngine.UI;

public class Tutorial3 : PopUpView
{
    [SerializeField] private Button backButton;

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(backButton != null, "Back button not set in how to view 3");
    }

    protected override void Awake()
    {
        base.Awake();

        backButton.onClick.AddListener(() => UI.PopOverlay());
    }

    public void OnBackClicked()
    {
        UI.PopOverlay();
    }
}
