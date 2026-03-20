using UnityEngine;
using UnityEngine.UI;

public class ProfileView : PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button exitButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(exitButton != null, "Exit button not set in profile view");
    }


    // ==================================================
    // Button Methods
    // ==================================================

    public void OnExitClicked()
    {
        UI.PopOverlay();
    }
}
