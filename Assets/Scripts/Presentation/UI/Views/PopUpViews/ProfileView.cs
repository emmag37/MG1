using UnityEngine;
using UnityEngine.UI;
using System;

public class ProfileView : PopUpView<IUserSettings>
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private InputField usernameInput;

    [SerializeField] private Button editAvatarButton;
    [SerializeField] private Image avatarImage;

    [SerializeField] private Button scoreHistoryButton;

    // ==================================================
    // Private Fields
    // ==================================================
    private string currentUsername;

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

        // update input text
        usernameInput.onSubmit.AddListener(Manager.UpdateUsername);
        // bring this back when you remove the legacy components
        /*
        usernameInput.onDeselect.AddListener(() =>
        {
            usernameInput.text = currentUsername;
        }); */ 

        editAvatarButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.ChooseAvatar));
        scoreHistoryButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.ScoreHistory));
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(IUserSettings data)
    {
        usernameInput.text = data.Username;
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);

        currentUsername = data.Username;

        base.Show(data);
    }

    public override void UpdateOverlay(IUserSettings data)
    {
        base.UpdateOverlay(data);

        usernameInput.text = data.Username;
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);

        currentUsername = data.Username;
    }
}
