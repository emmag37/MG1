using UnityEngine;
using UnityEngine.UI;
using System;

public class ProfileView : PopUpView<UserSettings>
{
    
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private InputField usernameInput;

    [SerializeField] private Button editAvatarButton;
    [SerializeField] private Image avatarImage;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(usernameInput != null, "Username input not set in profile view");

        Debug.Assert(editAvatarButton != null, "Edit avatar button not set in profile view");
        Debug.Assert(avatarImage != null, "Avatar image not set in profile view");
    }

    protected override void Awake()
    {
        base.Awake();

        usernameInput.onEndEdit.AddListener(Manager.UpdateUsername);

        editAvatarButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.ChooseAvatar)); 
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(UserSettings data)
    {
        usernameInput.text = data.Username;
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);

        base.Show(data);
    }

    public override void UpdateOverlay(UserSettings data)
    {
        base.UpdateOverlay(data);

        usernameInput.text = data.Username;
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);
    }

}
