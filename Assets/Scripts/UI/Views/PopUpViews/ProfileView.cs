using UnityEngine;
using UnityEngine.UI;
using System;

public class ProfileView : PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private InputField usernameInput;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(usernameInput != null, "Username input not set in profile view");
    }

    protected override void Awake()
    {
        base.Awake();

        // change this to on submit only?
        usernameInput.onEndEdit.AddListener(CheckUsername);
    }

    // ==================================================
    // Public Functions
    // ==================================================

    public void SetUsername(string name)
    {
        usernameInput.text = name;
    }

    // ==================================================
    // Private Functions
    // ==================================================

    private void CheckUsername(string name)
    {
        usernameInput.text = name;

        UI.RecieveUsername(name);
    }
}
