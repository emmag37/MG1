using UnityEngine;
using UnityEngine.UI;
using System;

public class ProfileView : PopUpView<PlayerProfile>
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

        usernameInput.onEndEdit.AddListener(UI.UpdateUsername);  // change to on submit only later
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(PlayerProfile data)
    {
        usernameInput.text = data.Username;

        base.Show(data);
    }

}
