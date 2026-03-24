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

        usernameInput.text = data.Profile.Username;

        usernameInput.onEndEdit.AddListener(data.SetUsername);  // change to on submit only later
    }

}
